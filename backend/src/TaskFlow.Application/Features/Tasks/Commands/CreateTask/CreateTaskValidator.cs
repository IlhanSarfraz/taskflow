using FluentValidation;

namespace TaskFlow.Application.Features.Tasks.Commands.CreateTask
{
    public sealed class CreateTaskValidator
    : AbstractValidator<CreateTaskCommand>
    {
        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(250);

            RuleFor(x => x.ProjectId)
                .NotEmpty();

            RuleFor(x => x.BoardColumnId)
                .NotEmpty();
        }
    }
}
