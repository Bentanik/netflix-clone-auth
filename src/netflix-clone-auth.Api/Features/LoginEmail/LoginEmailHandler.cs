using netflix_clone_auth.Api.Infrastructure.PasswordHash;

namespace netflix_clone_auth.Api.Features.LoginEmail;

public sealed class LoginEmailHandler : ICommandHandler<LoginEmailCommand, AuthLoginDto>
{
    private readonly ICommandRepository<User, Guid> _userRepo;
    private readonly IPasswordHashService _passwordHashService;

    public LoginEmailHandler(
        ICommandRepository<User, Guid> userRepo,
        IPasswordHashService passwordHashService)
    {
        _userRepo = userRepo;
        _passwordHashService = passwordHashService;
    }

    public async Task<Result<AuthLoginDto>> Handle(LoginEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepo.FindSingleAsync(
            predicate: u => u.Email == command.Email,
            isTracking: false,
            cancellationToken: cancellationToken);

        if (user == null)
        {
            var err = new Error(
                code: AuthMessages.InvalidCredentials.GetMessage().Code,
                message: AuthMessages.InvalidCredentials.GetMessage().Message);
            return Result.Failure<AuthLoginDto>([err]);
        }

        var verified = _passwordHashService.VerifyPassword(command.Password, user.PasswordHash);
        if (!verified)
        {
            var err = new Error(
                code: AuthMessages.InvalidCredentials.GetMessage().Code,
                message: AuthMessages.InvalidCredentials.GetMessage().Message);
            return Result.Failure<AuthLoginDto>([err]);
        }

        var authUserDto = new AuthUserDto(DisplayName: user.DisplayName, Email: user.Email);
        var authLoginDto = new AuthLoginDto(AuthUser: authUserDto);

        return Result.Success(
            data: authLoginDto, 
            code: AuthMessages.LoginSuccessfully.GetMessage().Code,
            message: AuthMessages.LoginSuccessfully.GetMessage().Message);
    }
}
