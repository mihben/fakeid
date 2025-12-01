using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Handlers;
using FakeID.Application.Test.Unit.Fixtures;
using Microsoft.Extensions.Logging;
using STrain.Core.Exceptions;

namespace FakeID.Application.Test.Unit
{
    public class CreateClientCommandTest : IClassFixture<DatabaseFixture>, IDisposable
    {
        private readonly ILogger<ClientPerformers> _logger;
        private readonly DatabaseFixture _fixture;

        public CreateClientCommandTest(DatabaseFixture fixture)
        {
            _logger = LoggerFactory.Create(builder => builder
                                                        .AddXUnit()
                                                        .SetMinimumLevel(LogLevel.Trace))
                                    .CreateLogger<ClientPerformers>();
            _fixture = fixture;
        }

        public ClientPerformers CreateSUT()
        {
            _fixture.OpenConnection();

            return new ClientPerformers(_fixture.CreateContext(), _logger);
        }

        public void Dispose()
        {
            _fixture.CloseConnection();
        }

        [Fact(DisplayName = "[UNIT][CCL-001]: Id is empty")]
        public async Task CreateClientCommand_ValidateAsync_IdIsEmpty()
        {
            // Arrange
            var sut = new CreateClientCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CreateClientCommandFaker().WithoutId().Generate());

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "[UNIT][CCL-002]: Name is empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task CreateClientCommand_ValidateAsync_NameIsEmpty(string? name)
        {
            // Arrange
            var sut = new CreateClientCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CreateClientCommandFaker().WithName(name).Generate());

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][CCL-003]: Create client")]
        public async Task CreateClientCommand_PerformAsync_CreateClient()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new CreateClientCommandFaker().Generate();

            // Act
            await sut.PerformAsync(command, CancellationToken.None);

            // Assert
            Assert.Collection(await _fixture.GetClientsAsync(), c => ClientAssert.Equal(command, c));
        }

        [Fact(DisplayName = "[UNIT][CCL-004]: Client has been created")]
        public async Task CreateClientCommand_PerformAsync_ClientIsExists()
        {
            // Arrange
            var sut = CreateSUT();
            var command = new CreateClientCommandFaker().Generate();

            await _fixture.InsertAsync(command.AsEntity());

            // Act
            // Assert
            await Assert.ThrowsAsync<VerificationException>(async () => await sut.PerformAsync(command, CancellationToken.None));
        }
    }

    file class CreateClientCommandFaker : AutoFaker<CreateClientCommand>
    {
        public CreateClientCommandFaker WithoutId()
        {
            RuleFor(c => c.Id, Guid.Empty);

            return this;
        }

        public CreateClientCommandFaker WithName(string? name)
        {
            RuleFor(c => c.Name, name);

            return this;
        }
    }

    file static class ClientAssert
    {
        public static void Equal(CreateClientCommand expected, ClientEntity value)
        {
            Assert.Equal(expected.Id, value.Id);
            Assert.Equal(expected.Name, value.Name);
        }
    }

    file static class CreateClientCommandTestExtensions
    {
        public static ClientEntity AsEntity(this CreateClientCommand command)
        {
            return new ClientEntity { Id = Guid.NewGuid(), Name = command.Name };
        }
    }
}
