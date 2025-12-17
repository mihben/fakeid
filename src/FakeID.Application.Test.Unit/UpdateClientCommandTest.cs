using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;
using STrain.Core.Exceptions;

namespace FakeID.Application.Test.Unit
{
    public class UpdateClientCommandTest : IClassFixture<ClientPerformersFixture>
    {
        private readonly ClientPerformersFixture _fixture;

        public UpdateClientCommandTest(ClientPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "[UNIT][UCC-001] - Id is empty")]
        public async Task UpdateClientCommandTest_ValidateAsync_IdIsEmpty()
        {
            // Arrange
            var sut = new UpdateClientCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithoutId().Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "[UNIT][UCC-002] - Name is empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task UpdateClientCommandTest_ValidateAsync_NameIsEmpty(string? name)
        {
            // Arrange
            var sut = new UpdateClientCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithName(name).Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][UCC-003] - Update Client")]
        public async Task UpdateClientCommandTest_PerformAsync_UpdateClient()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var command = new CommandFaker().Generate();

            await _fixture.InsertAsync(new CommandFaker().WithId(command.Id).Generate().AsEntity());

            // Act
            await sut.PerformAsync(command, default);

            // Assert
            Assert.Equal(command.Name, (await _fixture.GetAsync(command.Id))?.Name);
        }

        [Fact(DisplayName = "[UNIT][UCC-004] - Client does not exist")]
        public async Task UpdateClientCommandTest_PerformAsync_ClientDoesNotExist()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var command = new CommandFaker().Generate();

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await sut.PerformAsync(command, default));
        }

        [Fact(DisplayName = "[UNIT][UCC-005] - Client has already been added")]
        public async Task UpdateClientCommandTest_PerformAsync_ClientHasAlreadyBeenAdded()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var command = new CommandFaker().Generate();

            await _fixture.InsertAsync(new CommandFaker().WithName(command.Name).Generate().AsEntity());

            // Act
            // Assert
            await Assert.ThrowsAsync<VerificationException>(async () => await sut.PerformAsync(command, default));
        }
    }

    file class CommandFaker : AutoFaker<UpdateClientCommand>
    {
        public CommandFaker WithoutId()
        {
            RuleFor(c => c.Id, Guid.Empty);

            return this;
        }

        public CommandFaker WithId(Guid id)
        {
            RuleFor(c => c.Id, id);

            return this;
        }

        public CommandFaker WithName(string? name)
        {
            RuleFor(c => c.Name, name);

            return this;
        }
    }

    file static class UpdateClientCommandTestExtensions
    {
        public static ClientEntity AsEntity(this UpdateClientCommand command)
        {
            return new ClientEntity { Id = command.Id, Name = command.Name };
        }
    }
}
