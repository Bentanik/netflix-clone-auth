using netflix_clone_auth.Api.Infrastructure.PasswordHash;
using netflix_clone_auth.Api.Messages;

namespace netflix_clone_auth.Api.Features.RegisterEmail;

public sealed class RegisterEmailCommandHandler
    : ICommandHandler<RegisterEmailCommand>
{
    private readonly ICommandRepository<User, Guid> _userRepo;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterEmailCommandHandler
        (IPasswordHashService passwordHashService,
        ICommandRepository<User, Guid> userRepo,
        IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _passwordHashService = passwordHashService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RegisterEmailCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepo.AnyAsync(
            predicate: u => u.Email == command.Email,
            isTracking: false,
            cancellationToken: cancellationToken
        );


        if (existingUser)
        {
            var error = new Error
            (
                code: AuthMessages.EmailExist.GetMessage().Code,
                message: AuthMessages.EmailExist.GetMessage().Message
            );
            return Result.Failure([error]);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = command.DisplayName,
            Email = command.Email,
            PasswordHash = _passwordHashService.HashPassword(command.Password),
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
