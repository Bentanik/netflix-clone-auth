using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace netflix_clone_auth.Api.DependencyInjection.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        var authSettings = configuration.GetSection(AuthSettings.SectionName).Get<AuthSettings>();

        if (authSettings == null)
        {
            throw new ArgumentNullException(
                nameof(authSettings),
                $"Configuration section '{AuthSettings.SectionName}' is missing or invalid."
            );
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
       .AddJwtBearer(options =>
       {
           options.SaveToken = true;
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(authSettings.AccessSecretToken)),
               ClockSkew = TimeSpan.Zero
           };

           options.Events = new JwtBearerEvents
           {
               OnAuthenticationFailed = context =>
               {
                   if (context.Exception is SecurityTokenExpiredException)
                   {
                       if (!context.Response.Headers.ContainsKey("X-Token-Expired"))
                       {
                           context.Response.Headers.Append("X-Token-Expired", "true");
                       }
                   }
                   return Task.CompletedTask;
               }
           };
       });

        services.AddAuthorization();

        return services;
    }
}
