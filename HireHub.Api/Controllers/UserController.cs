using Azure.Core;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.DTO.Base;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using HireHub.Core.Validators;
using HireHub.Shared.Authentication.Filters;
using HireHub.Shared.Common.Exceptions;
using HireHub.Shared.Common.Models;
using HireHub.Shared.Middleware.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace HireHub.Api.Controllers;

[RequireAuth]
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly IUserProvider _userProvider;
    private readonly RepoService _repoService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<UserController> _logger;

    public UserController(UserService userService, IUserProvider userProvider,
        RepoService repoService, ITransactionRepository transactionRepository, 
        ILogger<UserController> logger)
    {
        _userService = userService;
        _userProvider = userProvider;
        _repoService = repoService;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }


    #region Get API's

    [RequireAuth([RoleName.Admin])]
    [HttpGet("fetch/all")]
    [ProducesResponseType<Response<List<UserDTO>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetUsers([FromQuery] string? role, [FromQuery] bool? isActive,
        [FromQuery] bool? isLatestFirst, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetUsers));

        try
        {
            object? userRole = null;
            if (role != null && !Enum.TryParse(typeof(UserRole), role, true, out userRole))
                throw new CommonException(ResponseMessage.InvalidRole);

            var response = await _userService.GetUsers(
                userRole != null ? (UserRole)userRole : null, 
                isActive, isLatestFirst, startDate, endDate, pageNumber, pageSize);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetUsers));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetUsers), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [HttpGet("fetch/{userId:int}")]
    [ProducesResponseType<Response<UserDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetUser([FromRoute] int userId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetUser));

        try
        {
            var response = await _userService.GetUser(userId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetUser));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetUser), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Mentor])]
    [HttpGet("fetchCandidatesDetail/{mentorId}")]
    [ProducesResponseType<Response<List<DriveCandidateDTO>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> getCandidatesByMentorId(int mentorId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(getCandidatesByMentorId));
        try
        {
            var response = await _userService.GetDrivewithCandidate(mentorId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(getCandidatesByMentorId));
            return Ok(response);

        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(getCandidatesByMentorId), ex.Message);
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

    [RequireAuth([RoleName.Admin])]
    [HttpPost("add")]
    [ProducesResponseType<Response<UserDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddUser));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    AddUserRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
                    .ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.Errors.ForEach(e =>
                        baseResponse.Errors.Add(new ValidationError
                        {
                            PropertyName = e.PropertyName,
                            ErrorMessage = e.ErrorMessage
                        })
                    );
                    return BadRequest(baseResponse);
                }

                var response = await _userService.AddUser(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(AddUser));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(AddUser), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }

    #endregion

    #region Put API's

    [RequireAuth([RoleName.Admin])]
    [HttpPut("edit")]
    [ProducesResponseType<Response<UserDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> EditUser([FromBody] JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditUser));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    EditUserRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
                    .ValidateAsync(request);

                if (!validator.IsValid)
                {
                    validator.Errors.ForEach(e =>
                        baseResponse.Errors.Add(new ValidationError
                        {
                            PropertyName = e.PropertyName,
                            ErrorMessage = e.ErrorMessage
                        })
                    );
                    return BadRequest(baseResponse);
                }

                var response = await _userService.EditUser(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(EditUser));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(EditUser), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }

    #endregion
}
