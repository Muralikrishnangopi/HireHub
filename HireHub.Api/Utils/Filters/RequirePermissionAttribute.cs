using HireHub.Core.Data.Interface;
using HireHub.Core.Utils.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HireHub.Api.Utils.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class RequirePermissionAttribute : TypeFilterAttribute
{
    public RequirePermissionAttribute(string userAction, string actionType)
        : base(typeof(RequirePermissionFilter))
    {
        Arguments = new object[] { userAction, actionType };
    }
}

public class RequirePermissionFilter : IAsyncAuthorizationFilter
{
    private readonly IUserPermissionRepository _userPermission;
    private readonly string _userAction;
    private readonly string _actionType;

    public RequirePermissionFilter(IUserPermissionRepository userPermission,
        string userAction, string actionType)
    {
        _userPermission = userPermission;
        _userAction = userAction;
        _actionType = actionType;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Get UserId from HttpContext.Items
        if (!context.HttpContext.Items.TryGetValue(Key.UserId, out var userIdObj))
        {
            context.HttpContext.Response.StatusCode = 401;
            context.Result = new ContentResult
            {
                StatusCode = 401,
                Content = "Unauthorized: UserId missing"
            };
            return;
        }

        var userId = int.Parse((string)userIdObj!);

        // Fetch permission from DB
        var permission = await _userPermission.GetUserPermissionAsync(userId, _userAction);

        if (permission == null)
        {
            context.HttpContext.Response.StatusCode = 403;
            context.Result = new ContentResult
            {
                StatusCode = 403,
                Content = "Forbidden: No permission record found"
            };
            return;
        }

        // Validate ActionType: "View", "Add", "Update", "Delete"
        bool allowed = _actionType.ToLower() switch
        {
            "view" => permission.View,
            "add" => permission.Add,
            "update" => permission.Update,
            "delete" => permission.Delete,
            _ => false
        };

        if (!allowed)
        {
            context.HttpContext.Response.StatusCode = 403;
            context.Result = new ContentResult
            {
                StatusCode = 403,
                Content = "Forbidden: Permission denied"
            };
        }
    }
}

