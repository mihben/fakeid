using FluentValidation;
using STrain;

namespace FakeID.Api
{
    public record UpdateClientCommand : Command
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
    }

    public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}
