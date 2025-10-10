namespace netflix_clone_auth.Api.Features.LoginEmail;

public class LoginEmailValidator : AbstractValidator<LoginEmailQuery>
{
    public LoginEmailValidator()
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
            .MaximumLength(100)
                .WithMessage("Password must not exceed 100 characters.");
    }
}