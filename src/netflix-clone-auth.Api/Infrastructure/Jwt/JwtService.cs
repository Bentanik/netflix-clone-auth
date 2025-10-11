namespace netflix_clone_auth.Api.Infrastructure.Jwt;

public class JwtService : IJwtService
{
    private readonly AuthSettings _settings;

    public JwtService(IOptions<AuthSettings> settings)
    {
        _settings = settings.Value;
    }

    public TokenResult GenerateAccessToken(UserDto user)
    {
        if (user.Id == null)
            throw new ArgumentNullException(nameof(user.Id), "User ID cannot be null");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.AccessSecretToken));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpMinute);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()!),
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResult(tokenString, expires);
    }

    public TokenResult GenerateRefreshToken(UserDto user, string jti)
    {
        if (user.Id == null)
            throw new ArgumentNullException(nameof(user.Id), "User ID cannot be null");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.RefreshSecretToken));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_settings.RefreshTokenExpMinute);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()!),
            new(JwtRegisteredClaimNames.Jti, jti)
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResult(tokenString, expires);
    }

    public ClaimsPrincipal? ValidateRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_settings.RefreshSecretToken);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            if (validatedToken is JwtSecurityToken jwt &&
                jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}