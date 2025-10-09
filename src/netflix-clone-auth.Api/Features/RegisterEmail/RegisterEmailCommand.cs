namespace netflix_clone_auth.Api.Features.RegisterEmail;

public record RegisterEmailCommand
    (string Email, string Password, string DisplayName) : ICommand;
