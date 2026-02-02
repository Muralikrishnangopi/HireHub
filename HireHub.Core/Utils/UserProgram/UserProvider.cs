using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HireHub.Core.Utils.UserProgram;

public class UserProvider : IUserProvider
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _http;
    private readonly ILogger<UserProvider> _logger;

    public UserProvider(IUserRepository userRepository,
        IHttpContextAccessor http, ILogger<UserProvider> logger)
    {
        _userRepository = userRepository;
        _http = http;
        _logger = logger;
    }

    public string CurrentUserId => GetCurrentUserId();

    private string GetCurrentUserId()
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetCurrentUserId));

        var userId = _http.HttpContext?.Items[Key.UserId]?.ToString() ?? throw new InvalidOperationException(ExceptionMessage.UserIdNotSetOnMiddleware);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetCurrentUserId));

        return userId;
    }

    public Task<User?> CurrentUser => GetCurrentUser();

    private async Task<User?> GetCurrentUser()
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetCurrentUser));

        var userId = _http.HttpContext?.Items[Key.UserId]?.ToString() ?? throw new InvalidOperationException(ExceptionMessage.UserIdNotSetOnMiddleware);

        var user = await _userRepository.GetByIdAsync(int.Parse(userId));

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetCurrentUser));

        return user;
    }

    public string CurrentUserRole => GetCurrentUserRole();

    private string GetCurrentUserRole()
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetCurrentUserRole));

        var role = _http.HttpContext?.Items[Key.Role]?.ToString() ?? throw new InvalidOperationException(ExceptionMessage.UserRoleNotSetOnMiddleware);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetCurrentUserRole));

        return role;
    }

}
