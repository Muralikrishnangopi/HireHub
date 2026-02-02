namespace HireHub.Core.DTO;

public class AddUserRequest
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string RoleName { get; set; } = null!;
    public string Password { get; set; } = null!;
}