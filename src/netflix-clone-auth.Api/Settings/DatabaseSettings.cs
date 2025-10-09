namespace netflix_clone_auth.Api.Settings;

public class DatabaseSettings
{
    public const string SectionName = "DatabaseSettings";
    public string ConnectionString { get; set; } = default!;
}