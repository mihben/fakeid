using FluentValidation;
using FluentValidation.Validators;
using STrain;

namespace FakeID.Api
{
    public record CreatePersonaCommand : Command
    {
        public Guid Client { get; init; }
        public Guid Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Role { get; init; }
    }

    public class CreatePersonaCommandValidator : AbstractValidator<CreatePersonaCommand>
    {
        public CreatePersonaCommandValidator()
        {
            RuleFor(c => c.Client).NotEmpty();
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.FirstName).NotEmpty();
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.Role).NotEmpty();
        }
    }
}
