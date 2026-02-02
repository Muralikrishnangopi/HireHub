using HireHub.Shared.Common.Models;

namespace HireHub.Core.DTO;

public class LoginRequest
{
    /// <summary>
    /// Accept email only here
    /// </summary>
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ForgotPasswordRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Otp {  get; set; } = null!;
}

public class ChangePasswordRequest
{
    public string Email { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}

public class VerifyEmailRequest
{
    public string Email { get; set; } = null!;
}
