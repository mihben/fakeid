namespace FakeID.Client.Blazor.Models
{
    public class Persona
    {
        public Guid Id { get; init; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
