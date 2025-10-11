namespace netflix_clone_auth.Api.Persistence.Repositories;

public interface IUserCommandRepository : ICommandRepository<User, Guid>
{
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
    Task<int[]?> CheckEmailAndDisplayNameAsync(string email, string displayName, CancellationToken cancellationToken = default);
}
