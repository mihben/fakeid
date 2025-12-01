using AutoBogus;
using Bogus.Extensions;
using FakeID.Api;
using FakeID.Client.Blazor.Services;
using Moq;
using STrain;
using STrain.CQS.Api;

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

        [Fact(DisplayName = "[UNIT][CDS-001] - Save client")]
        public async Task ClientDataService_SaveAsync_SaveClient()
        {
            // Arrange
            var sut = CreateSUT();
            var client = new AutoFaker<Models.Client>().Generate();

            // Act
            await sut.SaveAsync(client, default);

            // Assert
            _senderMock.VerifyCommand<CreateClientCommand>(c => c.Id == client.Id && c.Name == client.Name);

        }

        [Fact(DisplayName = "[UNIT][CDS-001] - Load Clients")]
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
    }

    file static class ClientDataServiceTestExtensions
    {
        public static Moq.Language.Flow.ISetup<IRequestSender, Task<T?>> SetupQuery<TQuery, T>(this Mock<IRequestSender> mock)
            where TQuery : IQuery
        {
            return mock.Setup(m => m.SendAsync<TQuery, T>(It.IsAny<TQuery>(), It.IsAny<CancellationToken>()));
        }

        public static void VerifyCommand<TCommand>(this Mock<IRequestSender> mock, Func<TCommand, bool> verify)
            where TCommand : ICommand
        {
            mock.Verify(m => m.SendAsync<TCommand, object?>(It.Is<TCommand>(c => verify(c)), It.IsAny<CancellationToken>()), Times.Once());
        }

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
    }
}
