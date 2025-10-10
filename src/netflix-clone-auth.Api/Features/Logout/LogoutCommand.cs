namespace netflix_clone_auth.Api.Features.Logout;

public record LogoutCommand(string RequestId, string UserId) : ICommand, IIdempotentRequest;
