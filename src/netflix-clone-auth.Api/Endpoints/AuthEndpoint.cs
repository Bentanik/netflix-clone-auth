using netflix_clone_auth.Api.Features.LoginEmail;
using netflix_clone_auth.Api.Features.Logout;
using netflix_clone_auth.Api.Features.RegisterEmail;

namespace netflix_clone_auth.Api.Endpoints;

public class AuthEndpoint : ICarterModule
{
    private const string BaseUrl = "/api/v{version:apiVersion}";

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.NewVersionedApi("Authentication")
            .MapGroup(BaseUrl).HasApiVersion(1);

        group.MapPost("/register-email", HandleRegisterEmailAsync);
        group.MapPost("/login-email", HandleLoginEmailAsync);
        group.MapDelete("/logout", HandleLogoutAsync).RequireAuthorization();
    }

    private static async Task<IResult> HandleRegisterEmailAsync(
        [FromServices] IMessageBus messageBus,
        [FromServices] IRequestContext requestContext,
        [FromBody] RegisterEmailRequest request)
    {
        string requestId = requestContext.GetIdempotencyKey()
            ?? throw new AppExceptions.XRequestIdRequiredException();

        var registerEmailCommand = new RegisterEmailCommand(
            RequestId: requestId,
            Email: request.Email,
            DisplayName: request.DisplayName,
            Password: request.Password
        );
        
        var result = await messageBus.Send(registerEmailCommand);

        return result.IsFailure ? Results.BadRequest(result) : Results.Ok(result);
    }

    private static async Task<IResult> HandleLoginEmailAsync(
      [FromServices] IMessageBus messageBus,
      [FromServices] IRequestContext requestContext,
      [FromServices] IHttpContextAccessor httpContextAccessor,
      [FromBody] LoginEmailRequest request)
    {
        if (httpContextAccessor.HttpContext?.Request.Headers.ContainsKey("Authorization") == true)
        {
            return Results.BadRequest(new
            {
                message = "Authorization header must not be provided when logging in."
            });
        }

        string requestId = requestContext.GetIdempotencyKey()
            ?? throw new AppExceptions.XRequestIdRequiredException();

        var loginEmailQuery = new LoginEmailQuery(
            RequestId: requestId,
            Email: request.Email,
            Password: request.Password
        );

        var result = await messageBus.Send(loginEmailQuery);

        return result.IsFailure ? Results.BadRequest(result) : Results.Ok(result);
    }


    private static async Task<IResult> HandleLogoutAsync(
      [FromServices] IMessageBus messageBus,
      [FromServices] IRequestContext requestContext,
      [FromServices] IHttpContextAccessor httpContextAccessor)
    {
        string requestId = requestContext.GetIdempotencyKey()
            ?? throw new AppExceptions.XRequestIdRequiredException();

        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Results.Unauthorized();

        var command = new LogoutCommand(
            RequestId: requestId,
            UserId: userId
        );

        var result = await messageBus.Send(command);

        return result.IsFailure ? Results.BadRequest(result) : Results.Ok(result);
    }
}
