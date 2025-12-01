using FluentValidation;
using STrain;

namespace FakeID.Api
{
    public record CreateClientCommand : Command
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
    }

    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}
