using HireHub.Core.Data.Interface;
using HireHub.Core.DTO;
using HireHub.Core.DTO.Base;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using HireHub.Shared.Authentication.Filters;
using HireHub.Shared.Common.Exceptions;
using HireHub.Shared.Common.Models;
using HireHub.Shared.Middleware.Models;
using Microsoft.AspNetCore.Mvc;

namespace HireHub.Api.Controllers;

[RequireAuth([RoleName.Admin])]
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly AdminService _adminService;
    private readonly IUserProvider _userProvider;
    private readonly RepoService _repoService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AdminService adminService, IUserProvider userProvider,
        RepoService repoService, ITransactionRepository transactionRepository,
        ILogger<AdminController> logger)
    {
        _adminService = adminService;
        _userProvider = userProvider;
        _repoService = repoService;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }


    #region Get API's

    [HttpGet("dashboard/details")]
    [ProducesResponseType<Response<AdminDashboardDetails>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetDashboardDetails()
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDashboardDetails));

        try
        {
            var response = await _adminService.GetDashboardDetails();

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetDashboardDetails));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetDashboardDetails), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }

    #endregion

    #region Post API's



    #endregion
}
