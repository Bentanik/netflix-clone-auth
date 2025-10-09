namespace netflix_clone_auth.Api.Persistence.Entitiy;

public class RefreshToken :  BaseEntity<Guid>
{
    public string IpAddress { get; set; } = null!;
    // Foreign key to User
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
