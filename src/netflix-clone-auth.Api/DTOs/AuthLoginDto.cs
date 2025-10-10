namespace netflix_clone_auth.Api.DTOs;

public record AuthLoginDto(TokenResult AuthToken, AuthUserDto AuthUser);
