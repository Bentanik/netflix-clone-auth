using System.Collections.Generic;

namespace netflix_clone_auth.Api.Persistence.Entitiy;

public class User : BaseEntity<Guid>
{
    public string DisplayName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsEmailConfirmed { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
