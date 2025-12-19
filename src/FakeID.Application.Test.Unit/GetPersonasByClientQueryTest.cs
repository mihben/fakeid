using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;

namespace FakeID.Application.Test.Unit
{
    public class GetPersonasByClientQueryTest : IClassFixture<PersonaPerformersFixture>
    {
        private readonly PersonaPerformersFixture _fixture;

        public GetPersonasByClientQueryTest(PersonaPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory(DisplayName = "[UNIT][GPC-001] - Client is Empty")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetPersonasByClientQuery_ValidateAsync_ClientIsEmpty(string? client)
        {
            // Arrange
            var sut = new GetPersonasByClientQueryValidator();

            // Act
            var result = await sut.ValidateAsync(new QueryFaker().WithClient(client).Generate(), default);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][GPC-002] - Get Personas")]
        public async Task GetPersonasByClientQuery_PerformAsync_GetPersonas()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var persona = new AutoFaker<PersonaEntity>().Generate();

            await _fixture.InsertAsync(persona);

            // Act
            var result = await sut.PerformAsync(new QueryFaker().WithClient(persona.Client.Name).Generate(), default);

            // Assert
            Assert.Single(result, p => persona.Id.Equals(p.Id) &&
                                    persona.FirstName.Equals(p.FirstName) &&
                                    persona.LastName.Equals(p.LastName) &&
                                    persona.Email.Equals(p.Email) &&
                                    persona.Role.Equals(p.Role));
        }
    }

    public class QueryFaker : AutoFaker<GetPersonasByClientQuery>
    {
        public QueryFaker WithClient(string? client)
        {
            RuleFor(q => q.Client, client);

            return this;
        }
    }
}
