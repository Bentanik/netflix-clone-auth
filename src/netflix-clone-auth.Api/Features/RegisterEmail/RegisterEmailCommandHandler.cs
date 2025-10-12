using netflix_clone_auth.Api.Infrastructure.PasswordHash;
using netflix_clone_auth.Api.Messages;

namespace netflix_clone_auth.Api.Features.RegisterEmail;

public sealed class RegisterEmailCommandHandler
    : ICommandHandler<RegisterEmailCommand>
{
    private readonly IUserCommandRepository _userRepo;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserSettings _userSettings;

    public RegisterEmailCommandHandler
        (IPasswordHashService passwordHashService,
        IUserCommandRepository userRepo,
        IUnitOfWork unitOfWork,
        IOptions<UserSettings> userSettings)
    {
        _userRepo = userRepo;
        _passwordHashService = passwordHashService;
        _unitOfWork = unitOfWork;
        _userSettings = userSettings.Value;
    }

    public async Task<Result<object>> Handle(RegisterEmailCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepo.CheckEmailAndDisplayNameAsync(command.Email, command.DisplayName, cancellationToken);

        if (existingUser is not null)
        {
            var errors = new List<Error<object>>(2);

            if (existingUser[0] == 1)
            {
                errors.Add(new Error<object>(
                    code: AuthMessages.EmailExist.GetMessage().Code,
                    message: AuthMessages.EmailExist.GetMessage().Message));
            }

            if (existingUser[1] == 1)
            {
                errors.Add(new Error<object>(
                    code: AuthMessages.DisplayNameExist.GetMessage().Code,
                    message: AuthMessages.DisplayNameExist.GetMessage().Message));
            }

            return Result.Failure(errors);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = command.DisplayName,
            Email = command.Email,
            PasswordHash = _passwordHashService.HashPassword(command.Password),
            AvatarId = _userSettings.AvatarId,
            AvatarUrl = _userSettings.AvatarUrl,
            IsEmailConfirmed = false
        };

        await _userRepo.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Todo: Send confirmation email

        return Result.Success(
            code: AuthMessages.RegisterSuccessfully.GetMessage().Code,
            message: AuthMessages.RegisterSuccessfully.GetMessage().Message);
    }
}
