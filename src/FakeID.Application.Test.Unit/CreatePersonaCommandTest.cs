using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;
using STrain.Core.Exceptions;
using Xunit;

namespace FakeID.Application.Test.Unit
{
    public class CreatePersonaCommandTest : IClassFixture<PersonaPerformersFixture>
    {
        private readonly PersonaPerformersFixture _fixture;

        public CreatePersonaCommandTest(PersonaPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "[UNIT][CPC-001] - Client is empty")]
		public async Task CreatePersonaCommand_ValidateAsync_ClientIsEmpty()
		{
			// Arrange
			var sut = new CreatePersonaCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new CreatePersonaCommandFaker().WithoutClient().Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}
    
		[Fact(DisplayName = "[UNIT][CPC-002] - Id is empty")]
		public async Task CreatePersonaCommand_ValidateAsync_IdIsEmpty()
		{
			// Arrange
			var sut = new CreatePersonaCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new CreatePersonaCommandFaker().WithoutId().Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}

		[Theory(DisplayName = "[UNIT][CPC-003] - First name is empty")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public async Task CreatePersonaCommand_ValidateAsync_FirstNameIsEmpty(string? name)
		{
			// Arrange
			var sut = new CreatePersonaCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new CreatePersonaCommandFaker().WithFirstName(name).Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}

		[Theory(DisplayName = "[UNIT][CPC-004] - Last name is empty")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public async Task CreatePersonaCommand_ValidateAsync_LastNameIsEmpty(string? name)
		{
			// Arrange
			var sut = new CreatePersonaCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new CreatePersonaCommandFaker().WithFirstName(name).Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}

		[Theory(DisplayName = "[UNIT][CPC-005] - Email is empty")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("  ")]
		public async Task CreatePersonaCommand_ValidateAsync_EmailIsEmpty(string? email)
		{
			// Arrange
			var sut = new CreatePersonaCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new CreatePersonaCommandFaker().WithEmail(email).Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}

		[Fact(DisplayName = "[UNIT][CPC-006] - With invalid email")]
		public async Task CreatePersonaCommand_ValidateAsync_WithInvalidEmail()
		{
			// Arrange
			var sut = new CreatePersonaCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new CreatePersonaCommandFaker().InvalidEmail().Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}

		[Theory(DisplayName = "[UNIT][CPC-007] - Without role")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public async Task CreatePersonaCommand_ValidateAsync_WithoutRole(string? role)
		{
			// Arrange
			var sut = new CreatePersonaCommandValidator();

			// Act
			var result = await sut.ValidateAsync(new CreatePersonaCommandFaker().WithRole(role).Generate(), default);

			// Assert
			Assert.False(result.IsValid);
		}

		[Fact(DisplayName = "[UNIT][CPC-008] - Create persona")]
		public async Task CreatePersonaCommand_PerformAsync_CreatePersona()
		{
			// Arrange
			var sut = _fixture.CreateSUT();
			var client = new AutoFaker<ClientEntity>().Generate();
			var command = new CreatePersonaCommandFaker().WithClient(client.Id).Generate();

			await _fixture.InsertAsync(client);

			// Act
			await sut.PerformAsync(command, default);

			// Assert
			Assert.Single(await _fixture.GetAsync(client.Id), p => command.Id == p.Id &&
															command.FirstName == p.FirstName &&
															command.LastName == p.LastName &&
															command.Email == p.Email &&
															command.Role == p.Role);
		}

		[Fact(DisplayName = "[UNIT][CPC-009] - Client not exist")]
		public async Task CreatePersonaCommand_PerformAsync_ClientNotExist()
		{
			// Arrange
			var sut = _fixture.CreateSUT();
			var command = new CreatePersonaCommandFaker().Generate();

			// Act
			// Assert
			await Assert.ThrowsAsync<NotFoundException>(async () => await sut.PerformAsync(command, default));
		}
    }

	file class CreatePersonaCommandFaker : AutoFaker<CreatePersonaCommand>
	{
        public CreatePersonaCommandFaker()
        {
			RuleFor(c => c.Email, f => f.Internet.Email());
        }

        public CreatePersonaCommandFaker WithoutClient()
		{
			RuleFor(c => c.Client, Guid.Empty);

			return this;
		}

        public CreatePersonaCommandFaker WithClient(Guid client)
		{
			RuleFor(c => c.Client, client);

			return this;
		}

		public CreatePersonaCommandFaker WithoutId()
		{
			RuleFor(c => c.Id, Guid.Empty);

			return this;
		}

		public CreatePersonaCommandFaker WithFirstName(string? name)
		{
			RuleFor(c => c.FirstName, name);

			return this;
		}

		public CreatePersonaCommandFaker WithLastName(string? name)
		{
			RuleFor(c => c.LastName, name);

			return this;
		}

		public CreatePersonaCommandFaker WithEmail(string? email)
		{
			RuleFor(c => c.Email, email);

			return this;
		}

		public CreatePersonaCommandFaker InvalidEmail()
		{
			RuleFor(c => c.Email, f => f.Random.String2(10));

			return this;
		}

		public CreatePersonaCommandFaker WithRole(string? role)
		{
			RuleFor(c => c.Role, role);

			return this;
		}
	}

	file static class CreatePersonaCommandTestExtensions
	{

    }
}
