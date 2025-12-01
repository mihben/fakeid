namespace FakeID.Client.Blazor.Models
{
    public record Client
    {
        public Guid Id { get; init; }
        public string? Name { get; set; }
    }
}
