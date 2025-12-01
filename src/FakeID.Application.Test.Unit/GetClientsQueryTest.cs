using AutoBogus;
using FakeID.Api;
using FakeID.Application.Entities;
using FakeID.Application.Test.Unit.Fixtures;

namespace FakeID.Application.Test.Unit
{
    public class GetClientsQueryTest : IClassFixture<ClientPerformersFixture>
    {
        private readonly ClientPerformersFixture _fixture;

        public GetClientsQueryTest(ClientPerformersFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact(DisplayName = "[UNIT][GCQ-001] - Get Clients")]
        public async Task GetClientsQuery_PerformAsync_GetClients()
        {
            // Arrange
            var sut = _fixture.CreateSUT();
            var client = new AutoFaker<ClientEntity>().Generate();

            await _fixture.InsertAsync(client);

            // Act
            var result = await sut.PerformAsync(new GetClientsQuery(), default);

            // Assert
            Assert.Collection(result, (c) => { Assert.Equal(client.Id, c.Id); Assert.Equal(client.Name, c.Name); });
        }
    }
}
