using FluentValidation;
using STrain;

namespace FakeID.Api
{
    public record DeletePersonaCommand : Command
    {
        public Guid Client { get; init; }
        public Guid Persona { get; init; }
    }

    public class DeletePersonaCommandValidator : AbstractValidator<DeletePersonaCommand>
    {
        public DeletePersonaCommandValidator()
        {
            RuleFor(c => c.Client).NotEmpty();
            RuleFor(c => c.Persona).NotEmpty();
        }
    }
}
