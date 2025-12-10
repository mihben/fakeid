using AutoBogus;
using Bogus.Extensions;
using FakeID.Api;
using FakeID.Client.Blazor.Services;
using FakeID.Client.Blazor.Test.Unit.Helpers;
using Moq;
using STrain;

namespace FakeID.Client.Blazor.Test.Unit.Services
{
    public class ClientDataServiceTest
    {
        private Mock<IRequestSender> _senderMock = null!;

        private ClientDataService CreateSUT()
        {
            _senderMock = new Mock<IRequestSender>();

            return new ClientDataService(_senderMock.Object);
        }

        [Fact(DisplayName = "[UNIT][CDS-001] - Create client")]
        public async Task ClientDataService_SaveAsync_CreateClient()
        {
            // Arrange
            var sut = CreateSUT();
            var client = new AutoFaker<Models.Client>().Generate();

            // Act
            await sut.SaveAsync(client, default);

            // Assert
            _senderMock.VerifyCommand<CreateClientCommand>(c => c.Id == client.Id && c.Name == client.Name);
        }

        [Fact(DisplayName = "[UNIT][CDS-002] - Load Clients")]
        public async Task ClientDataService_LoadAsync_LoadClients()
        {
            // Arrange
            var sut = CreateSUT();
            var clients = new AutoFaker<GetClientsQuery.Result>().GenerateBetween(1, 10);

            _senderMock.SetupQuery<GetClientsQuery, IEnumerable<GetClientsQuery.Result>>()
                .ReturnsAsync(clients);

            // Act
            await sut.LoadAsync(default);

            // Assert
            Assert.Collection(sut.Clients, [.. clients.AsInspectors()]);
        }

        [Fact(DisplayName = "[UNIT][CDS-003] - Delete Client")]
        public async Task ClientDataService_LoadAsync_DeleteClient()
        {
            // Arrange
            var sut = CreateSUT();
            var client = new AutoFaker<Models.Client>().Generate();

            sut.Selected = client;

            // Act
            await sut.DeleteAsync(default);

            // Assert
            _senderMock.VerifyCommand<DeleteClientCommand>(c => c.Id == client.Id);
        }

        [Fact(DisplayName = "[UNIT][CDS-004] - Update client")]
        public async Task ClientDataService_SaveAsync_UpdateClient()
        {
            // Arrange
            var sut = CreateSUT();
            var client = new AutoFaker<Models.Client>().Generate();

            _senderMock.SetupQuery<GetClientsQuery, IEnumerable<GetClientsQuery.Result>>()
                .ReturnsAsync([client.AsResult()]);

            await sut.LoadAsync(default);

            // Act
            await sut.SaveAsync(client, default);

            // Assert
            _senderMock.VerifyCommand<UpdateClientCommand>(c => c.Id == client.Id && c.Name == client.Name);
        }
    }

    file static class ClientDataServiceTestExtensions
    {
        public static IEnumerable<Action<Models.Client>> AsInspectors(this IEnumerable<GetClientsQuery.Result> results)
        {
            foreach (var result in results)
            {
                yield return c =>
                {
                    Assert.Equal(result.Id, c.Id);
                    Assert.Equal(result.Name, c.Name);
                };
            }
        }

        public static GetClientsQuery.Result AsResult(this Models.Client client)
        {
            return new GetClientsQuery.Result { Id = client.Id, Name = client.Name! };
        }
    }
}
