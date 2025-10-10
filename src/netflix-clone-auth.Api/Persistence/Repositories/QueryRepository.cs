namespace netflix_clone_auth.Api.Persistence.Repositories;

public class QueryRepository<TEntity> : IQueryRepository<TEntity> where TEntity : class
{
    private readonly IDbConnection _connection;

    public QueryRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    private string GetTableName()
        => $"\"{typeof(TEntity).Name}\""; // quote để PostgreSQL case-sensitive

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {GetTableName()}";
        return await _connection.QueryAsync<TEntity>(new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<TEntity>> FindAsync(string whereClause, object? parameters = null, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {GetTableName()} WHERE {whereClause}";
        return await _connection.QueryAsync<TEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task<TEntity?> FindOneAsync(string whereClause, object? parameters = null, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT * FROM {GetTableName()} WHERE {whereClause} LIMIT 1";
        return await _connection.QueryFirstOrDefaultAsync<TEntity>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsAsync(string whereClause, object? parameters = null, CancellationToken cancellationToken = default)
    {
        var sql = $"SELECT EXISTS (SELECT 1 FROM {GetTableName()} WHERE {whereClause})";
        return await _connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }
}
