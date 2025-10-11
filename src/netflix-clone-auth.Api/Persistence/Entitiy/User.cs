namespace netflix_clone_auth.Api.Persistence.Entitiy;

public class User : BaseEntity<Guid>
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string AvatarId { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public bool IsEmailConfirmed { get; set; }
}
