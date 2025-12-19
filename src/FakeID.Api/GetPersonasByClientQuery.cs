using FluentValidation;
using STrain;

namespace FakeID.Api
{
    public record GetPersonasByClientQuery : Query<IEnumerable<GetPersonasByClientQuery.Result>>
    {
        public required string Client { get; init; }

        public record Result
        {
            public Guid Id { get; init; }
            public required string FirstName { get; init; }
            public required string LastName { get; init; }
            public required string Email { get; init; }
            public required string Role { get; init; }
        }
    }

    public class GetPersonasByClientQueryValidator : AbstractValidator<GetPersonasByClientQuery>
    {
        public GetPersonasByClientQueryValidator()
        {
            RuleFor(q => q.Client).NotEmpty();
        }
    }
}
