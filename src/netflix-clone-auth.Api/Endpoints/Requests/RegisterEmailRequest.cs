namespace netflix_clone_auth.Api.Endpoints.Requests;

public record RegisterEmailRequest(string Email, string DisplayName, string Password);
