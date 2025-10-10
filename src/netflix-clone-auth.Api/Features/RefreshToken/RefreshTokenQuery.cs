namespace netflix_clone_auth.Api.Features.RefreshToken;

public record RefreshTokenQuery(string RequestId, string UserId) : IQuery<AuthLoginDto>, IIdempotentRequest;
