using FluentValidation;

namespace TaskFlow.Application.Features.Tasks.Commands.UpdateTask
{
    public sealed class UpdateTaskValidator
        : AbstractValidator<UpdateTaskCommand>
    {
        public UpdateTaskValidator()
        {
            RuleFor(x => x.TaskId)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(250);
        }
    }
}
