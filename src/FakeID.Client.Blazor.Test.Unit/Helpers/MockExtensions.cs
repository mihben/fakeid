using Moq;
using STrain;

namespace FakeID.Client.Blazor.Test.Unit.Helpers
{
    public static class MockExtensions
    {
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
