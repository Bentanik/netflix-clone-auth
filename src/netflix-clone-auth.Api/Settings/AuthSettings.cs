namespace netflix_clone_auth.Api.Settings;

public class AuthSettings
{
    public const string SectionName = "AuthSettings";
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string AccessSecretToken { get; set; } = default!;
    public string RefreshSecretToken { get; set; } = default!;
    public double AccessTokenExpMinute { get; set; }
    public double RefreshTokenExpMinute { get; set; }
}