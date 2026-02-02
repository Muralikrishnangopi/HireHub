using HireHub.Core.Data.Models;

namespace HireHub.Core.Utils.UserProgram.Interface;

public interface IUserProvider
{
    string CurrentUserId { get; }
    Task<User?> CurrentUser {  get; }
    string CurrentUserRole { get; }
}
