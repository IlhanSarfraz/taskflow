using FluentValidation;

namespace TaskFlow.Application.Features.Tasks.Commands.MoveTask
{
    public sealed class MoveTaskValidator
        : AbstractValidator<MoveTaskCommand>
    {
        public MoveTaskValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty();

            RuleFor(x => x.TaskId)
                .NotEmpty();
        }
    }
}
