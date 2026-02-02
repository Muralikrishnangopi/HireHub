using FluentValidation;
using HireHub.Core.DTO;

namespace HireHub.Core.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(List<object> warnings)
    {
        RuleFor(e => e.Username).NotEmpty();
        RuleFor(e => e.Password).NotEmpty();
    }
}

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator(List<object> warnings)
    {
        RuleFor(e => e.Email).NotEmpty();
        RuleFor(e => e.Password).NotEmpty();
        RuleFor(e => e.Otp).NotEmpty();
    }
}

public class ChangePasswordValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordValidator(List<object> warnings)
    {
        RuleFor(e => e.Email).NotEmpty();
        RuleFor(e => e.OldPassword).NotEmpty();
        RuleFor(e => e.NewPassword).NotEmpty();
    }
}

public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator(List<object> warnings)
    {
        RuleFor(x => x.Email).NotEmpty();
    }
}
