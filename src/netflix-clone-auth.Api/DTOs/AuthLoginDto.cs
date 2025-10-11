namespace netflix_clone_auth.Api.DTOs;

public record AuthLoginDto(TokenResult? AccessToken, TokenResult? RefreshToken, AuthUserDto? AuthUser);
