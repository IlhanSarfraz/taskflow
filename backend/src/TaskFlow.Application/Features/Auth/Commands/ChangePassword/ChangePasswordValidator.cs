using FluentValidation;

namespace TaskFlow.Application.Features.Auth.Commands.ChangePassword
{
    public sealed class ChangePasswordValidator
        : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}