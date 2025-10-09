using netflix_clone_auth.Api.Infrastructure.PasswordHash;

namespace netflix_clone_auth.Api.Features.RegisterEmail;

public sealed class RegisterEmailHandler
    : ICommandHandler<RegisterEmailCommand>
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHashService _passwordHashService;

    public RegisterEmailHandler(AppDbContext dbContext, IPasswordHashService passwordHashService)
    {
        _dbContext = dbContext;
        _passwordHashService = passwordHashService;
    }

    public async Task<Result> Handle(RegisterEmailCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            DisplayName = command.DisplayName,
            Email = command.Email,
            PasswordHash = _passwordHashService.HashPassword(command.Password),
            IsEmailConfirmed = false
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
