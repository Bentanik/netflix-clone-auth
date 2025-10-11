namespace netflix_clone_auth.Api.Messages;

public enum AuthMessages
{
    [Message("Register successfully", "AUTH_01")]
    RegisterSuccessfully,

    [Message("Email exist", "AUTH_02")]
    EmailExist,

    [Message("DisplayName exist", "AUTH_03")]
    DisplayNameExist,

    [Message("Email or password is incorrect", "AUTH_04")]
    InvalidCredentials,

    [Message("Login successfully", "AUTH_05")]
    LoginSuccessfully,

    [Message("Logout successfully", "AUTH_06")]
    LogoutSuccessfully,

    [Message("Refresh token successfully", "AUTH_07")]
    RefreshTokenSuccessfully,

    [Message("Refresh token not found or expired", "AUTH_08")]
    INVALID_TOKEN,

    [Message("Authorization must not be provided when logging in.", "AUTH_09")]
    AuthProvidedOnLogin
}