using netflix_clone_auth.Api.Features.LoginEmail;
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
      [FromBody] LoginEmailRequest request)
    {
        string requestId = requestContext.GetIdempotencyKey()
            ?? throw new AppExceptions.XRequestIdRequiredException();

        var registerEmailCommand = new LoginEmailQuery(
            RequestId: requestId,
            Email: request.Email,
            Password: request.Password
        );

        var result = await messageBus.Send(registerEmailCommand);

        return result.IsFailure ? Results.BadRequest(result) : Results.Ok(result);
    }

}
