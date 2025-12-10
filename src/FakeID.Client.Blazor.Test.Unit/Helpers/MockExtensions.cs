using Moq;
using STrain;
using STrain.CQS.Api;

namespace FakeID.Client.Blazor.Test.Unit.Helpers
{
    public static class MockExtensions
    {
        public static Moq.Language.Flow.ISetup<IRequestSender, Task<T?>> SetupQuery<TQuery, T>(this Mock<IRequestSender> mock)
            where TQuery : IQuery
        {
            return mock.Setup(m => m.SendAsync<TQuery, T>(It.IsAny<TQuery>(), It.IsAny<CancellationToken>()));
        }
        public static void VerifyCommand<TCommand>(this Mock<IRequestSender> mock, Func<TCommand, bool> verify)
            where TCommand : Command
        {
            mock.VerifyCommand(verify, Times.Once());
        }

        public static void VerifyCommand<TCommand>(this Mock<IRequestSender> mock, Func<TCommand, bool> verify, Times times)
            where TCommand : Command
        {
            mock.Verify(s => s.SendAsync<TCommand, object?>(It.Is<TCommand>(c => verify(c)), It.IsAny<CancellationToken>()), times);
        }
    }
}
