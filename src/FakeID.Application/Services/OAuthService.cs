using FakeID.Api;
using FakeID.Application.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using STrain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FakeID.Application.Services
{
    public class OAuthService : IOauthService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly IOptions<ApplicationOptions> _options;
        private readonly IRequestSender _sender;
        private readonly ISystemClock _clock;
        private readonly ILogger<OAuthService> _logger;

        public OAuthService(IHttpContextAccessor accessor, IOptions<ApplicationOptions> options, IRequestSender sender, ISystemClock clock, ILogger<OAuthService> logger)
        {
            _accessor = accessor;
            _options = options;
            _sender = sender;
            _clock = clock;
            _logger = logger;
        }

        public async Task AuthorizeAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Attempting to authorize request");

            var responseType = _accessor.HttpContext.Request.Query.Read("response_type");
            if (responseType != "code") throw new ArgumentOutOfRangeException("response_type", responseType);

            var clientId = _accessor.HttpContext.Request.Query.Read("client_id");

            var redirectUri = new Uri(_accessor.HttpContext.Request.Query.Read("redirect_uri"));

            var state = _accessor.HttpContext.Request.Query.Read("state");

            var user = _accessor.HttpContext.Request.Query.Read("user", false);
            if (string.IsNullOrEmpty(user))
            {
                _logger.LogDebug("User not found. Redirect to login page");
                _accessor.HttpContext.Response.Redirect($"{_options.Value.LoginEndpoint.AbsoluteUri}?response_type={responseType}&client_id={clientId}&redirect_uri={redirectUri}&state={state}");
            }
            else
            {
                _logger.LogDebug("User found. Redirect to application");

                var personas = await _sender.GetAsync<GetPersonasByClientQuery, IEnumerable<GetPersonasByClientQuery.Result>>(new GetPersonasByClientQuery(clientId!), cancellationToken);

                var code = personas.Single(p => p.Id == Guid.Parse(user)).EncodeCode("fakeid", clientId, _clock.UtcNow.DateTime);

                _accessor.HttpContext.Response.Redirect(new UriBuilder(redirectUri).Code(code).State(state).Uri.AbsoluteUri);
            }
        }
    }

    file static class OAuthServiceExtensions
    {
        public static string? Read(this IQueryCollection parameters, string key, bool mandatory = true)
        {
            if (!parameters.TryGetValue(key, out var values) && mandatory) throw new ArgumentNullException(key);

            return values.FirstOrDefault();
        }

        public static UriBuilder State(this UriBuilder builder, string? state)
        {
            if (string.IsNullOrWhiteSpace(state)) return builder;

            if (string.IsNullOrWhiteSpace(builder.Query)) builder.Query = "?";
            else builder.Query = $"{builder.Query}&";

            builder.Query = $"{builder.Query}state={state}";

            return builder;
        }

        public static UriBuilder Code(this UriBuilder builder, string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return builder;

            if (string.IsNullOrWhiteSpace(builder.Query)) builder.Query = "?";
            else builder.Query = $"{builder.Query}&";

            builder.Query = $"{builder.Query}code={code}";

            return builder;
        }

        public static string EncodeCode(this GetPersonasByClientQuery.Result persona, string issuer, string audience, DateTime now)
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
}
