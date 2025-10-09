namespace netflix_clone_auth.Api.Features.RegisterEmail;

public class RegisterEmailValidator : AbstractValidator<RegisterEmailCommand>
{
    public RegisterEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithErrorCode("EMAIL_REQUIRED")
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithErrorCode("EMAIL_INVALID")
                .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithErrorCode("PASSWORD_REQUIRED")
                .WithMessage("Password is required.")
            .MinimumLength(6)
                .WithErrorCode("PASSWORD_TOO_SHORT")
                .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100)
                .WithErrorCode("PASSWORD_TOO_LONG")
                .WithMessage("Password must not exceed 100 characters.");

        RuleFor(x => x.DisplayName)
            .NotEmpty()
                .WithErrorCode("DISPLAY_NAME_REQUIRED")
                .WithMessage("Display name is required.")
            .MaximumLength(50)
                .WithErrorCode("DISPLAY_NAME_TOO_LONG")
                .WithMessage("Display name must not exceed 50 characters.");
    }
}