using AutoBogus;
using Bogus.Extensions;
using FakeID.Api;
using FakeID.Client.Blazor.Services;
using FakeID.Client.Blazor.Test.Unit.Helpers;
using Moq;
using STrain;

namespace FakeID.Client.Blazor.Test.Unit.Services
{
    public class PersonaDataServiceTest
    {
        private Mock<IRequestSender> _senderMock = default!;

        private PersonaDataService CreateSUT()
        {
            _senderMock = new Mock<IRequestSender>();

            return new PersonaDataService(_senderMock.Object);
        }

        [Fact(DisplayName = "[UNIT][PDS-001] - Load Personas")]
        public async Task PersonaDataService_LoadAsync_LoadPersonas()
        {
            // Arrange
            var sut = CreateSUT();
            var personas = new AutoFaker<GetPersonasQuery.Result>().GenerateBetween(1, 10);

            _senderMock.SetupQuery<GetPersonasQuery, IEnumerable<GetPersonasQuery.Result>>().ReturnsAsync(personas);

            // Act
            await sut.LoadAsync(Guid.NewGuid(), default);

            // Assert
            Assert.Collection(sut.Personas, [.. personas.AsInspectors()]);
        }
    }

    file static class PersonalDataServiceExtensions
    {
        public static IEnumerable<Action<Models.Persona>> AsInspectors(this IEnumerable<GetPersonasQuery.Result> personas)
        {
            foreach (var persona in personas)
            {
                yield return (r) =>
                {
                    Assert.Equal(persona.Id, r.Id);
                    Assert.Equal(persona.FirstName, r.FirstName);
                    Assert.Equal(persona.LastName, r.LastName);
                    Assert.Equal(persona.Email, r.Email);
                    Assert.Equal(persona.Role, r.Role);
                };
            }
        }
    }
}
