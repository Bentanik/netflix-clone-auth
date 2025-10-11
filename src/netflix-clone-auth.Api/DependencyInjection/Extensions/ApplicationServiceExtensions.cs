using netflix_clone_auth.Api.Infrastructure.Jwt;
using netflix_clone_auth.Api.Infrastructure.PasswordHash;
using netflix_clone_auth.Api.Infrastructure.ResponseCache;
using StackExchange.Redis;

namespace netflix_clone_auth.Api.DependencyInjection.Extensions;

public static class ApplicationServiceExtensions
{
    // -------------------- AppSettings --------------------
    public static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<AuthSettings>(configuration.GetSection(AuthSettings.SectionName));
        services.Configure<RedisSettings>(configuration.GetSection(RedisSettings.SectionName));
    }

    // -------------------- Swagger & API Versioning --------------------
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer()
                .AddSwagger();

        services.AddApiVersioning(options => options.ReportApiVersions = true)
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

        return services;
    }

    // -------------------- Redis Services --------------------
    private static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSettings = configuration.GetSection(RedisSettings.SectionName).Get<RedisSettings>();

        if (redisSettings == null)
        {
            throw new ArgumentNullException(
                nameof(redisSettings),
                $"Configuration section '{RedisSettings.SectionName}' is missing or invalid."
            );
        }

        if (!redisSettings.Enabled)
            return services;

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

        services.AddSingleton<IResponseCacheService, ResponseCacheService>();

        return services;
    }

    // -------------------- Infrastructure --------------------
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IJwtService, JwtService>()
            .AddTransient<IPasswordHashService, PasswordHashService>()
            .AddScoped<IResponseCacheService, ResponseCacheService>();

        services.AddRedisServices(configuration);

        return services;
    }

    // -------------------- Persistence --------------------
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();

        if (dbSettings == null)
        {
            throw new ArgumentNullException(
                nameof(dbSettings),
                $"Configuration section '{DatabaseSettings.SectionName}' is missing or invalid."
            );
        }

        // EF Core
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(dbSettings.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });
        });

        // Dapper
        services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(dbSettings.ConnectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped(typeof(ICommandRepository<,>), typeof(CommandRepository<,>))
                .AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));

        return services;
    }

    // -------------------- Application Builder --------------------
    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // AppSettings
        builder.Services.AddAppSettings(builder.Configuration);

        // Swagger
        builder.Services.AddSwaggerServices();

        // Mediator
        builder.Services.AddMediator(AssemblyReference.Assembly);

        // Middleware
        builder.Services.AddScoped<ExceptionHandlingMiddleware>();
        builder.Services.AddIdempotenceRequest();

        // Context
        builder.Services.RegisterRequestContextServices();

        // Authentication + Authorization
        builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);

        // Carter
        builder.Services.AddCarter();

        // Infrastructure + Persistence
        builder.Services
            .AddInfrastructureServices(builder.Configuration)
            .AddPersistenceServices(builder.Configuration);

        return builder;
    }
}
