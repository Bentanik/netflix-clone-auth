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
    }

    private static async Task<IResult> HandleRegisterEmailAsync(
        [FromServices] IMessageBus messageBus,
        [FromBody] RegisterEmailRequest request)
    {
        var registerEmailCommand = new RegisterEmailCommand(
            Email: request.Email,
            DisplayName: request.DisplayName,
            Password: request.Password
        );
        
        var result = await messageBus.Send(registerEmailCommand);

        return result.IsFailure ? Results.BadRequest(result) : Results.Ok(result);
    }
}
