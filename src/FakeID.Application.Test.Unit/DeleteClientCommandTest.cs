using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;

namespace FakeID.Application.Test.Unit
{
    public class DeleteClientCommandTest : IClassFixture<ClientPerformersFixture>
    {
        private readonly ClientPerformersFixture _fixture;

        public DeleteClientCommandTest(ClientPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "[UNIT][DCC-001] - Id is empty")]
        public async Task DeleteClientCommand_ValidateAsync_IdIsEmptyAsync()
        {
            // Arrange
            var sut = new DeleteClientCommandValidator();
            var command = new DeleteClientCommandFaker().WithoutId().Generate();

            // Act
            var result = await sut.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][DCC-002] - Delete client")]
        public async Task DeleteClientCommand_PerformAsync_DeleteClient()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var client = new AutoFaker<ClientEntity>().Generate();

            await _fixture.InsertAsync(client);

            // Act
            await sut.PerformAsync(new DeleteClientCommand { Id = client.Id }, CancellationToken.None);

            // Assert
            Assert.Empty(await _fixture.GetClientsAsync());
        }

        [Fact(DisplayName = "[UNIT][DCC-003] - Client does not exist")]
        public async Task DeleteClientCommand_PerformAsync_ClientDoesNotExist()
        {
            // Arrange
            var sut = _fixture.CreateSUT();

            // Act
            await sut.PerformAsync(new DeleteClientCommandFaker().Generate(), CancellationToken.None);

            // Assert
            Assert.True(true);
        }
    }

    file class DeleteClientCommandFaker : AutoFaker<DeleteClientCommand>
    {
        public DeleteClientCommandFaker WithoutId()
        {
            RuleFor(c => c.Id, Guid.Empty);

            return this;
        }
    }
}
