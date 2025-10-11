using netflix_clone_auth.Api.Infrastructure.Jwt;
using netflix_clone_auth.Api.Infrastructure.ResponseCache;
using netflix_clone_auth.Api.Messages;

namespace netflix_clone_auth.Api.Features.RefreshToken;

public sealed class RefreshTokenQueryHandler : IQueryHandler<RefreshTokenQuery, AuthLoginDto>
{
    private readonly IQueryRepository<netflix_clone_auth.Api.Persistence.Entitiy.User> _userRepo;
    private readonly IJwtService _jwtService;
    private readonly IResponseCacheService _responseCacheService;

    public RefreshTokenQueryHandler(
        IQueryRepository<netflix_clone_auth.Api.Persistence.Entitiy.User> userRepo,
        IJwtService jwtService,
        IResponseCacheService responseCacheService)
    {
        _userRepo = userRepo;
        _jwtService = jwtService;
        _responseCacheService = responseCacheService;
    }

    public async Task<Result<AuthLoginDto>> Handle(RefreshTokenQuery query, CancellationToken cancellationToken)
    {
        // The query.UserId contains the user id (string) that was embedded in the refresh token.
        var userId = query.UserId;

        var parameters = new DynamicParameters();
        parameters.Add("@Id", Guid.Parse(userId));

        var user = await _userRepo.FindOneAsync(
            whereClause: "\"Id\" = @Id",
            parameters: parameters,
            cancellationToken: cancellationToken);

        if (user == null)
        {
            var err = new Error(
                code: AuthMessages.INVALID_TOKEN.GetMessage().Code,
                message: AuthMessages.INVALID_TOKEN.GetMessage().Message);
            return Result.Failure<AuthLoginDto>(new[] { err });
        }

        // Validate stored jti in cache
        var cachedJti = await _responseCacheService.GetAsync($"RefreshToken_{userId}");
        if (string.IsNullOrEmpty(cachedJti))
        {
            var err = new Error(
                code: AuthMessages.INVALID_TOKEN.GetMessage().Code,
                message: AuthMessages.INVALID_TOKEN.GetMessage().Message);
            return Result.Failure<AuthLoginDto>(new[] { err });
        }

        // Generate new tokens
        var userDto = new UserDto(user.Id.ToString());
        var newJti = Guid.NewGuid().ToString();
        var accessToken = _jwtService.GenerateAccessToken(userDto);
        var refreshToken = _jwtService.GenerateRefreshToken(userDto, newJti);

        var authUserDto = new AuthUserDto(user.DisplayName, user.Email);
        var authLoginDto = new AuthLoginDto(accessToken, authUserDto);

        // Update cache with new jti and expiry
        await _responseCacheService.SetAsync(
            key: $"RefreshToken_{userDto.Id}",
            value: newJti,
            expiry: refreshToken.ExpiresAt - DateTime.UtcNow
        );

        return Result.Success(
            data: authLoginDto,
            code: AuthMessages.LoginSuccessfully.GetMessage().Code,
            message: AuthMessages.LoginSuccessfully.GetMessage().Message);
    }
}
