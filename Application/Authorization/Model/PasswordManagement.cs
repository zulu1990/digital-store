namespace Application.Authorization.Model
{
    public record ForgetPasswordRequest(string Email);

    public record ResetPasswordRequest(string Password, string Email, Guid Token);

    public record ChangePasswordRequest(string OldPassword, string NewPassword);

}
