using AutoBogus;
using Bogus;
using FakeID.Api;
using FakeID.Application.Options;
using FakeID.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using STrain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit.Abstractions;

namespace FakeID.Application.Test.Unit
{
    public class OAuthServiceTest
    {
        private readonly ILogger<OAuthService> _logger;
        private Mock<IHttpContextAccessor> _accessorMock = default!;
        private Mock<IRequestSender> _senderMock = default!;
        private Mock<ISystemClock> _clockMock = default!;
        private ApplicationOptions _options = default!;

        public OAuthServiceTest(ITestOutputHelper outputHelper)
        {
            _logger = new LoggerFactory()
                          .AddXUnit(outputHelper)
                          .CreateLogger<OAuthService>();
        }

        private OAuthService CreateSUT()
        {
            _accessorMock = new Mock<IHttpContextAccessor>();
            _senderMock = new Mock<IRequestSender>();
            _clockMock = new Mock<ISystemClock>();

            _clockMock.SetupGet(c => c.UtcNow).Returns(new Faker().Date.RecentOffset());

            var optionsMock = new Mock<IOptions<ApplicationOptions>>();

            _options = new AutoFaker<ApplicationOptions>().Generate();

            optionsMock.SetupGet(o => o.Value).Returns(_options);

            return new OAuthService(_accessorMock.Object, optionsMock.Object, _senderMock.Object, _clockMock.Object, _logger);
        }

        [Fact(DisplayName = "[UNIT][OAS-001] - response_type is not defined")]
        public async Task OAuthService_AuthorizeAsync_ResponseTypeIsNotDefined()
        {
            // Arrange
            var sut = CreateSUT();

            _accessorMock.SetupHttpContext(new HttpContextFaker().WithoutResponseType().Generate());

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.AuthorizeAsync(default));
        }

        [Fact(DisplayName = "[UNIT][OAS-002] - Invalid response_type")]
        public async Task OAuthService_AuthorizeAsync_InvalidResponseType()
        {
            // Arrange
            var sut = CreateSUT();

            _accessorMock.SetupHttpContext(new HttpContextFaker().ResponseType(new Faker().Random.String()).Generate());

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sut.AuthorizeAsync(default));
        }

        [Fact(DisplayName = "[UNIT][OAS-003] - client_id is not defined")]
        public async Task OAuthService_AuthorizeAsync_ClientIdIsNotDefined()
        {
            // Arrange
            var sut = CreateSUT();

            _accessorMock.SetupHttpContext(new HttpContextFaker().WithoutClientId().Generate());

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.AuthorizeAsync(default));
        }

        [Fact(DisplayName = "[UNIT][OAS-004] - redirect_uri is not defined")]
        public async Task OAuthService_AuthorizeAsync_RedirectUriIsNotDefined()
        {
            // Arrange
            var sut = CreateSUT();

            _accessorMock.SetupHttpContext(new HttpContextFaker().WithoutRedirectUri().Generate());

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.AuthorizeAsync(default));
        }

        [Fact(DisplayName = "[UNIT][OAS-005] - Invalid redirect_uri")]
        public async Task OAuthService_AuthorizeAsync_InvalidRedirectUri()
        {
            // Arrange
            var sut = CreateSUT();

            _accessorMock.SetupHttpContext(new HttpContextFaker().RedirectUri(new Faker().Random.String()).Generate());

            // Act
            // Assert
            await Assert.ThrowsAsync<UriFormatException>(async () => await sut.AuthorizeAsync(default));
        }

        [Fact(DisplayName = "[UNIT][OAS-006] - Redirect to Login")]
        public async Task OAuthService_AuthorizeAsync_RedirectToLogin()
        {
            // Arrange
            var sut = CreateSUT();
            var context = new HttpContextFaker().Generate();

            _accessorMock.SetupHttpContext(context);

            // Act
            await sut.AuthorizeAsync(default);

            // Assert
            Assert.Equal(StatusCodes.Status302Found, context.Response.StatusCode);
            Assert.Equal(_options.LoginEndpoint.AbsoluteUri, context.Response.Headers["Location"]);
        }

        [Fact(DisplayName = "[UNIT][OAS-007] - Redirect to RedirectUri")]
        public async Task OAuthService_AuthorizeAsync_RedirectToRedirectUri()
        {
            // Arrange
            var sut = CreateSUT();
            var persona = new AutoFaker<GetPersonasByClientQuery.Result>().Generate();
            var redirectUri = new Faker().Internet.UrlWithPath();
            var state = new Faker().Random.Guid();
            var clientId = new Faker().Random.String();
            var context = new HttpContextFaker().RedirectUri(redirectUri).User(persona.Id).ClientId(clientId).State(state).Generate();

            _senderMock.SetupQuery<GetPersonasByClientQuery, IEnumerable<GetPersonasByClientQuery.Result>>().ReturnsAsync([persona]);
            _accessorMock.SetupHttpContext(context);

            // Act
            await sut.AuthorizeAsync(default);

            // Assert
            Assert.Equal(StatusCodes.Status302Found, context.Response.StatusCode);
            Assert.Equal($"{redirectUri}?code={persona.Encode("fakeid", clientId, _clockMock.Object.UtcNow.DateTime)}&state={state}", context.Response.Headers["Location"]);
        }
    }

    file static class OAuthServiceTestExtensions
    {
        public static Mock<IHttpContextAccessor> SetupHttpContext(this Mock<IHttpContextAccessor> mock, HttpContext context)
        {
            mock.SetupGet(m => m.HttpContext).Returns(context);

            return mock;
        }
        public static Moq.Language.Flow.ISetup<IRequestSender, Task<TResponse?>> SetupQuery<TQuery, TResponse>(this Mock<IRequestSender> mock)
            where TQuery : Query<TResponse>
        {
            return mock.Setup(m => m.SendAsync<TQuery, TResponse>(It.IsAny<TQuery>(), It.IsAny<CancellationToken>()));
        }

        public static string Encode(this GetPersonasByClientQuery.Result persona, string issuer, string audience, DateTime now)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, persona.Id.ToString())
            };

            var key = new SymmetricSecurityKey("AC5D3B68-851F-454A-A249-8E8D01A37807"u8.ToArray());
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: issuer, audience: audience, claims: claims, expires: now.AddMinutes(10), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    file class HttpContextFaker
    {
        private readonly Dictionary<string, string> _queryParameters = new()
        {
            ["response_type"] = "code",
            ["client_id"] = new Faker().Random.String(),
            ["redirect_uri"] = new Faker().Internet.UrlWithPath(),
            ["state"] = new Faker().Random.Guid().ToString()
        };

        private readonly Dictionary<string, string> _headers = new();

        public HttpContextFaker ResponseType(string responseType)
        {
            _queryParameters["response_type"] = responseType;

            return this;
        }

        public HttpContextFaker RedirectUri(string redirectUri)
        {
            _queryParameters["redirect_uri"] = redirectUri;

            return this;
        }

        public HttpContextFaker State(Guid state)
        {
            _queryParameters["state"] = state.ToString();

            return this;
        }

        public HttpContextFaker ClientId(string clientId)
        {
            _queryParameters["client_id"] = clientId;

            return this;
        }

        public HttpContextFaker User(Guid id)
        {
            _headers.Add("user", id.ToString());

            return this;
        }

        public HttpContextFaker WithoutResponseType()
        {
            _queryParameters.Remove("response_type");

            return this;
        }

        public HttpContextFaker WithoutClientId()
        {
            _queryParameters.Remove("client_id");

            return this;
        }

        public HttpContextFaker WithoutRedirectUri()
        {
            _queryParameters.Remove("redirect_uri");

            return this;
        }

        public HttpContext Generate()
        {
            var context = new DefaultHttpContext();

            context.Request.QueryString = QueryString.Create(_queryParameters);
            foreach (var header in _headers)
            {
                context.Request.Headers.Add(header.Key, header.Value);
            }

            return context;
        }
    }
}
