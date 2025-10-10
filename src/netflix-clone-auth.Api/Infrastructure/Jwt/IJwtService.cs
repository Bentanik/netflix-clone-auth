namespace netflix_clone_auth.Api.Infrastructure.Jwt;

public interface IJwtService
{
    TokenResult GenerateAccessToken(UserDto user);
    TokenResult GenerateRefreshToken(UserDto user, string jti);
    ClaimsPrincipal? ValidateAccessToken(string token);
}
