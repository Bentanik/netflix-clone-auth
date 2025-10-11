using netflix_clone_auth.Api.Infrastructure.Jwt;
using netflix_clone_auth.Api.Infrastructure.PasswordHash;
using netflix_clone_auth.Api.Infrastructure.ResponseCache;
using netflix_clone_auth.Api.Messages;

namespace netflix_clone_auth.Api.Features.LoginEmail;

public sealed class LoginEmailQueryHandler : IQueryHandler<LoginEmailQuery, AuthLoginDto>
{
    private readonly IQueryRepository<User> _userRepo;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IResponseCacheService _responseCacheService;

    public LoginEmailQueryHandler(
        IQueryRepository<User> userRepo,
        IPasswordHashService passwordHashService,
        IJwtService jwtService,
        IResponseCacheService responseCacheService)
    {
        _userRepo = userRepo;
        _passwordHashService = passwordHashService;
        _jwtService = jwtService;
        _responseCacheService = responseCacheService;
    }

    public async Task<Result<AuthLoginDto>> Handle(LoginEmailQuery query, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Email", query.Email);

        var user = await _userRepo.FindOneAsync(
            whereClause: "\"Email\" = @Email",
            parameters: parameters,
            cancellationToken: cancellationToken
        );

        if (user == null || _passwordHashService.VerifyPassword(query.Password, user.PasswordHash) == false)
        {
            var err = new Error(
                code: AuthMessages.InvalidCredentials.GetMessage().Code,
                message: AuthMessages.InvalidCredentials.GetMessage().Message);
            return Result.Failure<AuthLoginDto>([err]);
        }

        var userDto = new UserDto(Id: user.Id.ToString());
        var jti = Guid.NewGuid().ToString();
        var accessToken = _jwtService.GenerateAccessToken(userDto);
        var refreshToken = _jwtService.GenerateRefreshToken(userDto, jti);

        var authUserDto = new AuthUserDto(DisplayName: user.DisplayName, Email: user.Email);
        var authLoginDto = new AuthLoginDto(AuthToken: accessToken, AuthUser: authUserDto);

        await _responseCacheService.SetAsync(
            key: $"RefreshToken_{userDto.Id}",
            value: jti,
            expiry: refreshToken.ExpiresAt - DateTime.UtcNow
        );

        return Result.Success(
            data: authLoginDto, 
            code: AuthMessages.LoginSuccessfully.GetMessage().Code,
            message: AuthMessages.LoginSuccessfully.GetMessage().Message);
    }
}
