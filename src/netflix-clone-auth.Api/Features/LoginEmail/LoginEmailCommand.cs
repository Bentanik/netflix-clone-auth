namespace netflix_clone_auth.Api.Features.LoginEmail;

public record LoginEmailCommand(string RequestId, string Email, string Password) : ICommand<AuthLoginDto>, IIdempotentRequest;
