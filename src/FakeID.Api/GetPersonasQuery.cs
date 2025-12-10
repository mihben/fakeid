using FluentValidation;
using STrain;

namespace FakeID.Api
{
    public record GetPersonasQuery : Query<IEnumerable<GetPersonasQuery.Result>>
    {
        public Guid Client { get; init; }

        public GetPersonasQuery(Guid client)
        {
            Client = client;
        }

        public record Result
        {
            public Guid Id { get; init; }
            public required string FirstName { get; init; }
            public required string LastName { get; init; }
            public required string Email { get; init; }
            public required string Role { get; init; }
        }
    }

    public class GetPersonasQueryValidator : AbstractValidator<GetPersonasQuery>
    {
        public GetPersonasQueryValidator()
        {
            RuleFor(q => q.Client).NotEmpty();
        }
    }
}
