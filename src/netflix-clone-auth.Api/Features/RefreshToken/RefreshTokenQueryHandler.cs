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
        var claims = _jwtService.ValidateRefreshToken(query.RefreshToken);
        if(claims == null)
        {
            return Result.Failure<AuthLoginDto>([
                new Error(
                    code: AuthMessages.INVALID_TOKEN.GetMessage().Code,
                    message: AuthMessages.INVALID_TOKEN.GetMessage().Message)
            ]);
        }

        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier);
        var jti = claims.FindFirstValue(JwtRegisteredClaimNames.Jti);

        // Validate stored jti in cache
        var cachedJti = await _responseCacheService.GetAsync($"RefreshToken_{userId}");
        if (userId == null || string.IsNullOrEmpty(cachedJti) || !cachedJti.Equals(jti))
        {
            var err = new Error(
                code: AuthMessages.INVALID_TOKEN.GetMessage().Code,
                message: AuthMessages.INVALID_TOKEN.GetMessage().Message);
            return Result.Failure<AuthLoginDto>([err]);
        }

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

        // Generate new tokens
        var userDto = new UserDto(user.Id.ToString());
        var newJti = Guid.NewGuid().ToString();
        var accessToken = _jwtService.GenerateAccessToken(userDto);
        var refreshToken = _jwtService.GenerateRefreshToken(userDto, newJti);

        var authUserDto = new AuthUserDto(user.DisplayName, user.Email, user.AvatarUrl);
        var authLoginDto = new AuthLoginDto(AccessToken: accessToken, RefreshToken: refreshToken, AuthUser: authUserDto);

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
