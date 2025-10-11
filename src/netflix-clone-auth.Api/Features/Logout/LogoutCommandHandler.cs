using netflix_clone_auth.Api.Infrastructure.ResponseCache;
using netflix_clone_auth.Api.Messages;

namespace netflix_clone_auth.Api.Features.Logout;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IResponseCacheService _responseCacheService;

    public LogoutCommandHandler(
        IResponseCacheService responseCacheService)
    {
        _responseCacheService = responseCacheService;
    }

    public async Task<Result> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        await _responseCacheService.RemoveAsync(
            key: $"RefreshToken_{command.UserId}"
        );

        return Result.Success(
            code: AuthMessages.LogoutSuccessfully.GetMessage().Code,
            message: AuthMessages.LogoutSuccessfully.GetMessage().Message);
    }
}
