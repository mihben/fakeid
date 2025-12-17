using FluentValidation;
using STrain;

namespace FakeID.Api
{
    public record DeleteClientCommand : Command
    {
        public Guid Id { get; init; }
    }

    public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
    {
        public DeleteClientCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
        }
    }
}
