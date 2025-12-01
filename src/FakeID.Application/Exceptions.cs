using STrain.Core.Exceptions;

namespace FakeID.Application
{
    public static class Exceptions
    {
        public static class Clients
        {
            public static VerificationException AlreadyExist(string name)
            {
                return new VerificationException("CLI001", "Already Exists", $"Client with name {name} has already been created");
            }
        }
    }
}
