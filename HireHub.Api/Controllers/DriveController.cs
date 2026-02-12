using HireHub.Api.Utils.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.DTO.Base;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using HireHub.Core.Validators;
using HireHub.Infrastructure.Repositories;
using HireHub.Shared.Authentication.Filters;
using HireHub.Shared.Common.Exceptions;
using HireHub.Shared.Common.Models;
using HireHub.Shared.Middleware.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace HireHub.Api.Controllers;

[RequireAuth]
[Route("api/[controller]")]
[ApiController]
public class DriveController : ControllerBase
{
    private readonly DriveService _driveService;
    private readonly IUserProvider _userProvider;
    private readonly RepoService _repoService;
    private readonly CommonService _commonService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<DriveController> _logger;
    private readonly RoundService _roundService;
       

    public DriveController(DriveService driveService, IUserProvider userProvider,
        RepoService repoService, CommonService commonService,
        ITransactionRepository transactionRepository, ILogger<DriveController> logger,
        RoundService roundService
        )
    {
        _driveService = driveService;
        _userProvider = userProvider;
        _repoService = repoService;
        _commonService = commonService;
        _transactionRepository = transactionRepository;
        _logger = logger;
        _roundService = roundService;
    }


    #region Get API's

    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [HttpGet("fetch/all")]
    [ProducesResponseType<Response<List<DriveDTO>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetDrives([FromQuery] string? driveStatus,
        [FromQuery] string? creatorEmail, [FromQuery] int? technicalRounds, [FromQuery] bool? isLatestFirst, 
        [FromQuery] bool includePastDrives, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDrives));

        try
        {
            object? status = null;
            if (driveStatus != null && !Enum.TryParse(typeof(DriveStatus), driveStatus, true, out status))
                throw new CommonException(ResponseMessage.InvalidDriveStatus);

            var response = await _driveService.GetDrives(
                status != null ? (DriveStatus)status : null,
                creatorEmail, technicalRounds, isLatestFirst, includePastDrives,
                startDate, endDate, pageNumber, pageSize);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetDrives));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetDrives), ex.Message);
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [HttpGet("fetch/{driveId:int}")]
    [ProducesResponseType<Response<DriveDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetDrive([FromRoute] int driveId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDrive));

        try
        {
            var response = await _driveService.GetDrive(driveId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetDrive));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetDrive), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [HttpGet("config/fetch/{driveId:int}")]
    [ProducesResponseType<Response<DriveConfigDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetDriveConfig([FromRoute] int driveId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDriveConfig));

        try
        {
            var response = await _driveService.GetDriveConfig(driveId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetDriveConfig));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetDriveConfig), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [HttpGet("member/fetch/all")]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [ProducesResponseType<Response<List<DriveMemberDTO>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetDriveMembers([FromQuery] int? driveId, [FromQuery] int? userId, [FromQuery] string? role,
        [FromQuery] string? driveStatus, [FromQuery] bool? isLatestFirst, [FromQuery] bool includePastDrives, 
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDriveMembers));

        try
        {
            object? userRole = null;
            if (role != null && !Enum.TryParse(typeof(UserRole), role, true, out userRole))
                throw new CommonException(ResponseMessage.InvalidRole);
            object? status = null;
            if (driveStatus != null && !Enum.TryParse(typeof(DriveStatus), driveStatus, true, out status))
                throw new CommonException(ResponseMessage.InvalidDriveStatus);

            var response = await _driveService.GetDriveMembers(driveId, userId, 
                userRole != null ? (UserRole)userRole : null, status != null ? (DriveStatus)status : null, 
                isLatestFirst, includePastDrives, startDate, endDate, pageNumber, pageSize);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetDriveMembers));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetDriveMembers), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [HttpGet("candidate/fetch/all")]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [ProducesResponseType<Response<List<DriveCandidateDTO>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetDriveCandidates([FromQuery] int? driveId, [FromQuery] int? candidateId, [FromQuery] string? candidateStatus,
        [FromQuery] string? driveStatus, [FromQuery] bool? isLatestFirst, [FromQuery] bool includePastDrives,
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDriveCandidates));

        try
        {
            object? candidateStatusObj = null;
            if (candidateStatus != null && !Enum.TryParse(typeof(CandidateStatus), candidateStatus, true, out candidateStatusObj))
                throw new CommonException(ResponseMessage.InvalidCandidateStatus);
            object? driveStatusObj = null;
            if (driveStatus != null && !Enum.TryParse(typeof(DriveStatus), driveStatus, true, out driveStatusObj))
                throw new CommonException(ResponseMessage.InvalidDriveStatus);

            var response = await _driveService.GetDriveCandidates(driveId, candidateId,
                candidateStatusObj != null ? (CandidateStatus)candidateStatusObj : null, 
                driveStatusObj != null ? (DriveStatus)driveStatusObj : null,
                isLatestFirst, includePastDrives, startDate, endDate, pageNumber, pageSize);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetDriveCandidates));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetDriveCandidates), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [HttpGet("round/fetch/all")]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [ProducesResponseType<Response<List<RoundDTO>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetInterviewRounds([FromQuery] int? driveId, [FromQuery] int? userId, 
        [FromQuery] string? roundType, [FromQuery] string? roundStatus, [FromQuery] string? roundResult,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetInterviewRounds));

        try
        {
            object? roundTypeObj = null;
            if (roundType != null && !Enum.TryParse(typeof(RoundType), roundType, true, out roundTypeObj))
                throw new CommonException(ResponseMessage.InvalidRoundType);
            object? roundStatusObj = null;
            if (roundStatus != null && !Enum.TryParse(typeof(RoundStatus), roundStatus, true, out roundStatusObj))
                throw new CommonException(ResponseMessage.InvalidRoundStatus);
            object? roundResultObj = null;
            if (roundResult != null && !Enum.TryParse(typeof(RoundResult), roundResult, true, out roundResultObj))
                throw new CommonException(ResponseMessage.InvalidRoundResult);

            var response = await _driveService.GetInterviewRounds(driveId, userId,
                roundTypeObj != null ? (RoundType)roundTypeObj : null, 
                roundStatusObj != null ? (RoundStatus)roundStatusObj : null,
                roundResultObj != null ? (RoundResult)roundResultObj : null,
                pageNumber, pageSize);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetInterviewRounds));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetInterviewRounds), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [HttpGet("round/fetch/{interviewRoundId:int}")]
    [ProducesResponseType<Response<RoundDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetInterviewRound([FromRoute] int interviewRoundId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetInterviewRound));

        try
        {
            var response = await _driveService.GetInterviewRound(interviewRoundId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetInterviewRound));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetInterviewRound), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.View)]
    [HttpGet("feedback/fetch/{feedbackId:int}")]
    [ProducesResponseType<Response<FeedbackDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetFeedback([FromRoute] int feedbackId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetFeedback));

        try
        {
            var response = await _driveService.GetFeedback(feedbackId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetFeedback));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetFeedback), ex.Message);
            return BadRequest(new BaseResponse()
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [HttpGet("candidate/template/bulk-upload")]
    [ProducesResponseType<FileContentResult>(200)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ErrorResponse>(500)]
    public IActionResult DownloadBulkUploadTemplate()
    {
        return File(
            TemplateService.CandidateBulkUploadTemplate.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "Candidate_Bulk_Upload_Template.xlsx"
        );
    }

    #endregion

    #region Post API's

    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Add)]
    [HttpPost("create")]
    [ProducesResponseType<Response<DriveDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> CreateDrive([FromBody] CreateDriveRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(CreateDrive));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new CreateDriveRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var currentUserId = int.Parse(_userProvider.CurrentUserId);
                var response = await _driveService.CreateDriveAsync(request, currentUserId);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(CreateDrive));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(CreateDrive), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPost("candidates/add")]
    [ProducesResponseType<Response<List<int>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> AddCandidatesToDrive([FromBody] AddCandidatesToDriveRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddCandidatesToDrive));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new AddCandidatesToDriveRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _driveService.AddCandidatesToDriveAsync(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(AddCandidatesToDrive));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(AddCandidatesToDrive), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
        catch (DbUpdateException ex) 
        when (ex.InnerException != null && ex.InnerException.Message.Contains(InnerExceptionMessage.DriveCandidateAlreadyExistViolation))
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(AddCandidatesToDrive), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ResponseMessage.SomeCandidateAlreadyAddedToDrive }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPost("candidate/upload/bulk")]
    [ProducesResponseType<Response<List<int>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> DriveCandidateBulkUpload([FromQuery] int driveId, IFormFile file)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(DriveCandidateBulkUpload));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var drive = await _repoService.DriveRepository.GetByIdAsync(driveId) ??
                    throw new CommonException(ResponseMessage.DriveNotFound);

                # region Adding Candidates

                var addCandidateRequests = await ExcelMapper.ExtractAsync<AddCandidateRequest>(file);

                var validator = await new
                    BulkCandidateInsertRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
                    .ValidateAsync(addCandidateRequests);

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

                var insertCandidateBulkResponse = await _commonService.InsertCandidatesBulk(addCandidateRequests);

                # endregion

                # region Adding Drive Candidates

                var addCandidatesToDriveRequest = new AddCandidatesToDriveRequest
                {
                    DriveId = driveId,
                    CandidateIds = insertCandidateBulkResponse.Data!
                };

                validator = await new AddCandidatesToDriveRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
                    .ValidateAsync(addCandidatesToDriveRequest);

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

                var response = await _driveService.AddCandidatesToDriveAsync(addCandidatesToDriveRequest);

                # endregion

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(DriveCandidateBulkUpload));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(DriveCandidateBulkUpload), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
        catch (DbUpdateException ex)
        when (ex.InnerException != null && ex.InnerException.Message.Contains(InnerExceptionMessage.DriveCandidateAlreadyExistViolation))
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(AddCandidatesToDrive), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ResponseMessage.SomeCandidateAlreadyAddedToDrive }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPost("member/add")]
    [ProducesResponseType<Response<DriveMemberDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> AddMemberToDrive([FromBody] AddMemberToDriveRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddMemberToDrive));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new AddMemberToDriveRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _driveService.AddMemberToDriveAsync(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(AddMemberToDrive));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(AddMemberToDrive), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPost("feedback/add")]
    [ProducesResponseType<Response<FeedbackDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> AddFeedback([FromBody] AddFeedbackRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddFeedback));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new AddFeedbackRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var currentUserId = int.Parse(_userProvider.CurrentUserId);
                var response = await _driveService.AddFeedback(request, currentUserId);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(AddFeedback));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(AddFeedback), ex.Message);
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
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPut("edit")]
    [ProducesResponseType<Response<DriveDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> EditDrive([FromBody] JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditDrive));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    EditDriveRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _driveService.EditDrive(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(EditDrive));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(EditDrive), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPut("config/edit")]
    [ProducesResponseType<Response<DriveConfigDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> EditDriveConfig([FromBody] JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditDriveConfig));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    EditDriveConfigRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _driveService.EditDriveConfig(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(EditDriveConfig));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(EditDriveConfig), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPut("candidate/edit")]
    [ProducesResponseType<Response<DriveCandidateDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> EditDriveCandidate([FromBody] JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditDriveCandidate));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    EditDriveCandidateRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var currentUserId = int.Parse(_userProvider.CurrentUserId);
                var response = await _driveService.EditDriveCandidate(request, currentUserId);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(EditDriveCandidate));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(EditDriveCandidate), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPut("round/edit")]
    [ProducesResponseType<Response<RoundDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> EditInterviewRound([FromBody] JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditInterviewRound));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    EditInterviewRoundRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var currentUserId = int.Parse(_userProvider.CurrentUserId);
                var response = await _driveService.EditInterviewRound(request, currentUserId);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(EditInterviewRound));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(EditInterviewRound), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPut("feedback/edit")]
    [ProducesResponseType<Response<FeedbackDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> EditFeedback([FromBody] JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditFeedback));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    EditFeedbackRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var currentUserId = int.Parse(_userProvider.CurrentUserId);
                var response = await _driveService.EditFeedback(request, currentUserId);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(EditFeedback));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(EditFeedback), ex.Message);
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

    #region Delete API's

    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpDelete("member/remove")]
    [ProducesResponseType<Response<DriveMemberDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> RemoveDriveMember([FromBody] RemoveDriveMemberRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(RemoveDriveMember));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    RemoveDriveMemberRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _driveService.RemoveDriveMember(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(RemoveDriveMember));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(RemoveDriveMember), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [RequireAuth([RoleName.Admin])]
    [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpDelete("candidates/remove")]
    [ProducesResponseType<Response<List<int>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> RemoveDriveCandidates([FromBody] RemoveDriveCandidatesRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(RemoveDriveCandidates));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    RemoveDriveCandidatesRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _driveService.RemoveCandidatesFromDrive(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(RemoveDriveCandidates));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(RemoveDriveCandidates), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }
    
    [RequireAuth([RoleName.Admin, RoleName.Hr])]
    // [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPost("AutoAssign/panelmember")]
    [ProducesResponseType<Response<AutoAssignDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ErrorResponse>(500)]

    public async Task<IActionResult> AutoAssignPanel([FromQuery] int driveId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(driveId));
        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new AutoPanelAssignRequest(baseResponse.Warnings, _repoService, _userProvider)
                    .ValidateAsync(driveId);
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
                var response = await _roundService.AutoPanelAssign(driveId);
                _transactionRepository.CommitTransaction();
                //var response = await _roundService.AutoPanelAssign(driveId, currentUserId);
                return Ok();

            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(driveId), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    
    }
    [RequireAuth([RoleName.Admin, RoleName.Hr])]
    // [RequirePermission(UserAction.Drive, ActionType.Update)]
    [HttpPut("ReAssign/panelmember")]
    [ProducesResponseType<Response<ReassignPanel>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> ReassignInterviewer(ReassignPanel request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(ReassignInterviewer));
        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new ReassignPanelAssignRequest(baseResponse.Warnings, _repoService, _userProvider)
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
                await _roundService.ReassignInterviewer(request.roundId, request.oldInterviewId, request.newInterviewerId);
                _transactionRepository.CommitTransaction();
                return Ok();
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(ReassignInterviewer), ex.Message);
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