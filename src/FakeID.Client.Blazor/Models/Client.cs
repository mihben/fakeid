namespace FakeID.Client.Blazor.Models
{
    public record Client
    {
        public bool Selected { get; set; }
        public Guid Id { get; init; }
        public string? Name { get; set; }
    }
}
