namespace netflix_clone_auth.Api.Features.LoginEmail;

public record LoginEmailQuery(string RequestId, string Email, string Password) : IQuery<AuthLoginDto>, IIdempotentRequest;
