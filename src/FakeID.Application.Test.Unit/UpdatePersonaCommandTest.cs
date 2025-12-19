using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;
using STrain.Core.Exceptions;

namespace FakeID.Application.Test.Unit
{
    public class UpdatePersonaCommandTest : IClassFixture<PersonaPerformersFixture>
    {
        private readonly PersonaPerformersFixture _fixture;

        public UpdatePersonaCommandTest(PersonaPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "[UNIT][UPC-001] - Client is Empty")]
        public async Task UpdatePersonaCommand_ValidateAsync_ClientIsEmpty()
        {
            // Arrange
            var sut = new UpdatePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithEmptyClient().Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][UPC-002] - Persona is Empty")]
        public async Task UpdatePersonaCommand_ValidateAsync_PersonaIsEmpty()
        {
            // Arrange
            var sut = new UpdatePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithEmptyPersona().Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "[UNIT][UPC-003] - FirstName is Empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdatePersonaCommand_ValidateAsync_FirstNameIsEmpty(string? firstName)
        {
            // Arrange
            var sut = new UpdatePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithFirstName(firstName).Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "[UNIT][UPC-004] - LastName is Empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdatePersonaCommand_ValidateAsync_LastNameIsEmpty(string? lastName)
        {
            // Arrange
            var sut = new UpdatePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithLastName(lastName).Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "[UNIT][UPC-005] - Email is Empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdatePersonaCommand_ValidateAsync_EmailIsEmpty(string? email)
        {
            // Arrange
            var sut = new UpdatePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithEmail(email).Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "[UNIT][UPC-006] - Role is Empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdatePersonaCommand_ValidateAsync_RoleIsEmpty(string? role)
        {
            // Arrange
            var sut = new UpdatePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithRole(role).Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Theory(DisplayName = "[UNIT][UPC-007] - Username is Empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task UpdatePersonaCommand_ValidateAsync_UsernameIsEmpty(string? username)
        {
            // Arrange
            var sut = new UpdatePersonaCommandValidator();

            // Act
            var result = await sut.ValidateAsync(new CommandFaker().WithUsername(username).Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][UPC-100] - Update Persona")]
        public async Task UpdatePersonaCommand_PerformAsync_UpdatePesona()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var persona = new EntityFaker().Generate();

            await _fixture.InsertAsync(new EntityFaker().BasedOn(persona).Generate());

            // Act
            await sut.PerformAsync(persona.AsCommand(), default);

            // Assert
            var entity = await _fixture.GetPersonaAsync(persona.Id);
            Assert.Equal(entity!.FirstName, persona.FirstName);
            Assert.Equal(entity!.Username, persona.Username);
            Assert.Equal(entity!.LastName, persona.LastName);
            Assert.Equal(entity!.Email, persona.Email);
            Assert.Equal(entity!.Role, persona.Role);
        }

        [Fact(DisplayName = "[UNIT][UPC-101] - Update not Existing Persona")]
        public async Task UpdatePersonaCommand_PerformAsync_UpdateNotExistingPesona()
        {
            // Arrange
            var sut = _fixture.CreateSUT();

            // Act
            // Assert
            await Assert.ThrowsAsync<NotFoundException>(async () => await sut.PerformAsync(new CommandFaker().Generate(), default));
        }
        [Fact(DisplayName = "[UNIT][UPC-102] - Username has already been exists")]
        public async Task UpdatePersonaCommand_PerformAsync_UsernameHasAlreadyBeenExists()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var persona = new EntityFaker().Generate();

            await _fixture.InsertAsync(new EntityFaker().WithClient(persona.Client).WithUsername(persona.Username).Generate());

            // Act
            // Arrange
            await Assert.ThrowsAsync<VerificationException>(async () => await sut.PerformAsync(new CommandFaker().WithClient(persona.Client.Id).WithUsername(persona.Username).Generate(), default));
        }
    }

    file class EntityFaker : AutoFaker<PersonaEntity>
    {
        public EntityFaker BasedOn(PersonaEntity persona)
        {
            WithClient(persona.Client);
            RuleFor(p => p.Id, persona.Id);

            return this;
        }

        public EntityFaker WithClient(ClientEntity client)
        {
            RuleFor(p => p.Client, client);

            return this;
        }

        public EntityFaker WithUsername(string username)
        {
            RuleFor(p => p.Username, username);

            return this;
        }
    }

    file class CommandFaker : AutoFaker<UpdatePersonaCommand>
    {
        public CommandFaker WithClient(Guid client)
        {
            RuleFor(c => c.Client, client);

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

        public CommandFaker WithFirstName(string? firstName)
        {
            RuleFor(c => c.FirstName, firstName);

            return this;
        }

        public CommandFaker WithLastName(string? lastName)
        {
            RuleFor(c => c.LastName, lastName);

            return this;
        }

        public CommandFaker WithEmail(string? email)
        {
            RuleFor(c => c.Email, email);

            return this;
        }

        public CommandFaker WithRole(string? role)
        {
            RuleFor(c => c.Role, role);

            return this;
        }

        public CommandFaker WithUsername(string? username)
        {
            RuleFor(c => c.Username, username);

            return this;
        }
    }

    public static class UpdatePersonaCommandTestExtensions
    {
        public static UpdatePersonaCommand AsCommand(this PersonaEntity persona)
        {
            return new UpdatePersonaCommand
            {
                Client = persona.Client.Id,
                Persona = persona.Id,
                Username = persona.Username,
                FirstName = persona.FirstName,
                LastName = persona.LastName,
                Email = persona.Email,
                Role = persona.Role
            };
        }
    }
}
