namespace netflix_clone_auth.Api.Persistence.Repositories;

public class UserCommandRepository : CommandRepository<User, Guid>, IUserCommandRepository 
{
    public UserCommandRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Checks whether the specified <paramref name="email"/> or <paramref name="displayName"/> already exists in the system.
    /// </summary>
    /// <param name="email">The email address to check for duplication.</param>
    /// <param name="displayName">The display name to check for duplication.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// An array of two integers representing the existence of each field:
    /// <list type="bullet">
    ///   <item><description>[0] = 1 if the email already exists, otherwise 0.</description></item>
    ///   <item><description>[1] = 1 if the display name already exists, otherwise 0.</description></item>
    /// </list>
    /// Returns <c>null</c> if neither the email nor the display name exists.
    /// </returns>
    public async Task<int[]?> CheckEmailAndDisplayNameAsync(
        string email,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        // Query users whose Email or DisplayName matches the given values
        var result = await _context.Users
            .AsNoTracking()
            .Where(u => u.Email == email || u.DisplayName == displayName)
            .Select(u => new { u.Email, u.DisplayName })
            .ToListAsync(cancellationToken);

        // No match found
        if (result.Count == 0)
            return null;

        int emailExists = result.Any(u => u.Email == email) ? 1 : 0;
        int displayNameExists = result.Any(u => u.DisplayName == displayName) ? 1 : 0;

        return [emailExists, displayNameExists];
    }
}
