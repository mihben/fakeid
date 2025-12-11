using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;

namespace FakeID.Application.Test.Unit
{
    public class DeletePersonaCommandTest : IClassFixture<PersonaPerformersFixture>
    {
        private readonly PersonaPerformersFixture _fixture;

        public DeletePersonaCommandTest(PersonaPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "[UNIT][DLP-001] - Client is Empty")]
        public async Task DeletePersonaCommand_ValidateAsync_ClientIsEmpty()
        {
            // Arrange
            var sut = new DeletePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithEmptyClient().Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][DLP-002] - Persona is Empty")]
        public async Task DeletePersonaCommand_ValidateAsync_PersonaIsEmpty()
        {
            // Arrange
            var sut = new DeletePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithEmptyPersona().Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][DLP-003] - Delete Persona")]
        public async Task DeletePersonacommand_PerformAsync_DeletePersona()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var client = new AutoFaker<ClientEntity>().Generate();
            var persona = new AutoFaker<PersonaEntity>().RuleFor(e => e.Client, client).Generate();

            await _fixture.InsertAsync(persona);

            // Act
            await sut.PerformAsync(new CommandFaker().WithClient(client.Id).WithPersona(persona.Id).Generate(), default);

            // Assert
            Assert.Null(await _fixture.GetPersonaAsync(persona.Id));
        }

        [Fact(DisplayName = "[UNIT][DLP-004] - Delete Persona does not exist")]
        public async Task DeletePersonacommand_PerformAsync_DeletePersonaDoesNotExist()
        {
            // Arrange
            var sut = _fixture.CreateSUT();

            // Act
            await sut.PerformAsync(new CommandFaker().Generate(), default);

            // Assert
            Assert.True(true);
        }
    }

    file class CommandFaker : AutoFaker<DeletePersonaCommand>
    {
        public CommandFaker WithClient(Guid client)
        {
            RuleFor(c => c.Client, client);

            return this;
        }

        public CommandFaker WithPersona(Guid persona)
        {
            RuleFor(c => c.Persona, persona);

            return this;
        }

        public CommandFaker WithEmptyClient()
        {
            RuleFor(c => c.Client, Guid.Empty);

            return this;
        }

        public CommandFaker WithEmptyPersona()
        {
            RuleFor(c => c.Persona, Guid.Empty);

            return this;
        }
    }
}
