using STrain.Core.Exceptions;

namespace FakeID.Application
{
    public static class Exceptions
    {
        public static class Clients
        {
            public static NotFoundException NotFound = new("Client does not exist");

            public static VerificationException AlreadyExist(string name)
            {
                return new VerificationException("CLI001", "Already Exists", $"Client with name {name} has already been created");
            }
        }

        public static class Personas
        {
            public static NotFoundException NotFound = new("Persona does not exist");

            public static VerificationException Conflict(string username)
            {
                return new VerificationException("PRS001", "Already Exists", $"Persona with username {username} has already been created");
            }
        }
    }
}
