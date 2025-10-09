using netflix_clone_auth.Api.Infrastructure.PasswordHash;

namespace netflix_clone_auth.Api.DependencyInjection.Extensions;

public static class ApplicationServiceExtensions
{
    private static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwagger();

        services
           .AddApiVersioning(options => options.ReportApiVersions = true)
           .AddApiExplorer(options =>
           {
               options.GroupNameFormat = "'v'VVV";
               options.SubstituteApiVersionInUrl = true;
           });

        return services;
    }

    private static IServiceCollection AddPersistenceService(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = new DatabaseSettings();
        configuration.Bind(DatabaseSettings.SectionName, dbSettings);
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));

        var connectionString = dbSettings.ConnectionString;

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(dbSettings.ConnectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });
        });

        services
         .AddScoped<IUnitOfWork, UnitOfWork>()
         .AddScoped(typeof(ICommandRepository<,>), typeof(CommandRepository<,>))
         .AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));

        return services;
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IPasswordHashService, PasswordHashService>();
        return services;
    }

    public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSwaggerService();
        
        builder.Services.AddMediator(AssemblyReference.Assembly);
        
        builder.Services.AddScoped<ExceptionHandlingMiddleware>();
        
        builder.Services.AddIdempotenceRequest();
        
        builder.Services.AddCarter();

        builder.Services
            .AddInfrastructureServices()
            .AddPersistenceService(builder.Configuration);

        builder.Services.RegisterRequestContextServices();

        return builder;
    }
}
