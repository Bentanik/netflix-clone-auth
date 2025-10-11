namespace netflix_clone_auth.Api.Messages;

public enum AuthMessages
{
    [Message("Register successfully", "AUTH_01")]
    RegisterSuccessfully,

    [Message("Email exist", "AUTH_02")]
    EmailExist,

    [Message("Email or password is incorrect", "AUTH_03")]
    InvalidCredentials,

    [Message("Login successfully", "AUTH_04")]
    LoginSuccessfully,

    [Message("Logout successfully", "AUTH_05")]
    LogoutSuccessfully,

    [Message("Refresh token successfully", "AUTH_06")]
    RefreshTokenSuccessfully,

    [Message("Refresh token not found or expired", "AUTH_07")]
    INVALID_TOKEN
}