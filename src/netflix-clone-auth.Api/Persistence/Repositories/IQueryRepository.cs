namespace netflix_clone_auth.Api.Persistence.Repositories;

public interface IQueryRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> FindAsync(string whereClause, object? parameters = null, CancellationToken cancellationToken = default);
    Task<TEntity?> FindOneAsync(string whereClause, object? parameters = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string whereClause, object? parameters = null, CancellationToken cancellationToken = default);
}