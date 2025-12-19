using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;

namespace FakeID.Application.Test.Unit
{
    public class GetPersonasQueryTest : IClassFixture<PersonaPerformersFixture>
    {
        private readonly PersonaPerformersFixture _fixture;

        public GetPersonasQueryTest(PersonaPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "[UNIT][GPQ-001] - Client is empty")]
        public async Task GetPersonasQuery_ValidateAsync_ClientIsEmpty()
        {
            // Arrange
            var sut = new GetPersonasQueryValidator();
            var query = new GetPersonasQueryFaker().WithoutClient().Generate();

            // Act
            var result = await sut.ValidateAsync(query);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact(DisplayName = "[UNIT][GPQ-005] - Get Personas")]
        public async Task GetPersonasQuery_PerformAsync_GetPersonas()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var client = new AutoFaker<ClientEntity>().Generate();
            var persona = new PersonaEntityFaker().WithClient(client).Generate();

            await _fixture.InsertAsync(persona);

            // Act
            var result = await sut.PerformAsync(new GetPersonasQueryFaker().WithClient(client.Id).Generate(), default);

            // Assert
            Assert.Single(result, p => p.Id == persona.Id &&
                                    p.Username == persona.Username &&
                                    p.FirstName == persona.FirstName &&
                                    p.LastName == persona.LastName &&
                                    p.Email == persona.Email &&
                                    p.Role == persona.Role);
        }
    }

    file class GetPersonasQueryFaker : AutoFaker<GetPersonasQuery>
    {
        public GetPersonasQueryFaker WithClient(Guid id)
        {
            RuleFor(q => q.Client, id);

            return this;
        }

        public GetPersonasQueryFaker WithoutClient()
        {
            RuleFor(q => q.Client, Guid.Empty);

            return this;
        }
    }

    file class PersonaEntityFaker : AutoFaker<PersonaEntity>
    {
        public PersonaEntityFaker WithClient(ClientEntity client)
        {
            RuleFor(e => e.Client, client);

            return this;
        }
    }
}
