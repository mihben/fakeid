namespace FakeID.Application.Options
{
    public record ApplicationOptions
    {
        public required Uri LoginEndpoint { get; init; }
    }
}
