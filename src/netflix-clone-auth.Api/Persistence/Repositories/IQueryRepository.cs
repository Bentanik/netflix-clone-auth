namespace netflix_clone_auth.Api.Persistence.Repositories;

public interface IQueryRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<TEntity>> FindAsync(
        Func<TEntity, bool> predicate,
        CancellationToken cancellationToken = default
    );
}