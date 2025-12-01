using AutoBogus;
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
    }

    file static class ClientDataServiceTestExtensions
    {
        public static void VerifyCommand<TCommand>(this Mock<IRequestSender> mock, Func<TCommand, bool> verify)
            where TCommand : ICommand
        {
            mock.Verify(m => m.SendAsync<TCommand, object?>(It.Is<TCommand>(c => verify(c)), It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
