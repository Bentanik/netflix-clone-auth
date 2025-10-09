namespace netflix_clone_auth.Api.Features.RegisterEmail;

public record RegisterEmailCommand
    (string RequestId, string Email, string Password, string DisplayName) : ICommand, IIdempotentRequest;
