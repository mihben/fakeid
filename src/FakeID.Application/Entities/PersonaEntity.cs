namespace FakeID.Application.Entities
{
    public class PersonaEntity
    {
        public Guid Id { get; set; }
        public ClientEntity Client { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
