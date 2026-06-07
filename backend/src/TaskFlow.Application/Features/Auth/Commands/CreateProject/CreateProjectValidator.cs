using FluentValidation;

namespace TaskFlow.Application.Features.Auth.Commands.CreateProject
{
    public sealed class CreateProjectValidator
        : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Key)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
}
