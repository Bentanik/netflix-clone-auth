namespace netflix_clone_auth.Api.Features.RegisterEmail;

public class RegisterEmailValidator : AbstractValidator<RegisterEmailCommand>
{
    public RegisterEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Email is required.")
            .EmailAddress()
                .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password is required.")
            .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(50)
                .WithMessage("Password must not exceed 50 characters.");

        RuleFor(x => x.DisplayName)
            .NotEmpty()
                .WithMessage("Display name is required.")
            .MaximumLength(50)
                .WithMessage("Display name must not exceed 50 characters.");
    }
}