using STrain;

namespace FakeID.Api
{
    public record GetClientsQuery : Query<IEnumerable<GetClientsQuery.Result>>
    {

        public class Result
        {
            public Guid Id { get; init; }
            public required string Name { get; init; }
        }
    }
}
