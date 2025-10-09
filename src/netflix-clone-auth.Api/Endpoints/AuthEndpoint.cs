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

    private static async Task<IResult> HandleRegisterEmailAsync([FromServices] IMessageBus messageBus)
    {
        return Results.Ok("Register Email");
    }
}
