namespace netflix_clone_auth.Api.DTOs;

public record TokenResult(string Token, DateTime ExpiresAt, string? TokenType = "Bearer");
