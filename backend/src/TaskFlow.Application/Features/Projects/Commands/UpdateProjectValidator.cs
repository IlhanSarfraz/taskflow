using FluentValidation;

namespace TaskFlow.Application.Features.Projects.Commands
{
    public sealed class UpdateProjectValidator
        : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
}
