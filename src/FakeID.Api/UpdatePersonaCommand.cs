using FluentValidation;
using STrain;

namespace FakeID.Api
{
    public record UpdatePersonaCommand : Command
    {
        public Guid Client { get; init; }
        public Guid Persona { get; init; }
        public required string Username { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string Email { get; init; }
        public required string Role { get; init; }
    }

    public class UpdatePersonaCommandValidator : AbstractValidator<UpdatePersonaCommand>
    {
        public UpdatePersonaCommandValidator()
        {
            RuleFor(c => c.Client).NotEmpty();
            RuleFor(c => c.Persona).NotEmpty();
            RuleFor(c => c.Username).NotEmpty();
            RuleFor(c => c.FirstName).NotEmpty();
            RuleFor(c => c.LastName).NotEmpty();
            RuleFor(c => c.Email).NotEmpty();
            RuleFor(c => c.Role).NotEmpty();
        }
    }
}
