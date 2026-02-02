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
public class CandidateController : ControllerBase
{
    private readonly CandidateService _candidateService;
    private readonly IUserProvider _userProvider;
    private readonly RepoService _repoService;
    private readonly CommonService _commonService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<CandidateController> _logger;

    public CandidateController(CandidateService candidateService, IUserProvider userProvider,
        RepoService repoService, CommonService commonService,
        ITransactionRepository transactionRepository, ILogger<CandidateController> logger)
    {
        _candidateService = candidateService;
        _userProvider = userProvider;
        _repoService = repoService;
        _commonService = commonService;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }


    #region Get API's

    [RequireAuth([RoleName.Admin])]
    [HttpGet("fetch/all")]
    [ProducesResponseType<Response<List<CandidateDTO>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetCandidates([FromQuery] string? experienceLevel,
        [FromQuery] bool isLatestFirst, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate,
        [FromQuery] int? pageNumber, [FromQuery] int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetCandidates));

        try
        {
            object? expLevel = null;
            if (experienceLevel != null && !Enum.TryParse(typeof(CandidateExperienceLevel), experienceLevel, true, out expLevel))
                throw new CommonException(ResponseMessage.InvalidExperienceLevel);

            var response = await _candidateService.GetCandidates(
                expLevel != null ? (CandidateExperienceLevel)expLevel : null, 
                isLatestFirst, startDate, endDate, pageNumber, pageSize);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetCandidates));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetCandidates), ex.Message);
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [HttpGet("template/bulk-upload")]
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


    [RequireAuth([RoleName.Admin])]
    [HttpGet("fetch/{candidateId:int}")]
    [ProducesResponseType<Response<CandidateDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> GetCandidate([FromRoute] int candidateId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetCandidate));

        try
        {
            var response = await _candidateService.GetCandidate(candidateId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(GetCandidate));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(GetCandidate), ex.Message);
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
    [ProducesResponseType<Response<CandidateDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> AddCandidate([FromBody] AddCandidateRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddCandidate));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    AddCandidateRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _candidateService.AddCandidate(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(AddCandidate));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(AddCandidate), ex.Message);
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
    [HttpPost("upload/bulk")]
    [ProducesResponseType<Response<List<int>>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> CandidateBulkUpload(IFormFile file)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(CandidateBulkUpload));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var request = await ExcelMapper.ExtractAsync<AddCandidateRequest>(file);

                var validator = await new
                    BulkCandidateInsertRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _commonService.InsertCandidatesBulk(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(CandidateBulkUpload));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(CandidateBulkUpload), ex.Message);
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
    [ProducesResponseType<Response<CandidateDTO>>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ContentResult>(401)]
    [ProducesResponseType<ContentResult>(403)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> EditCandidate([FromBody] JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditCandidate));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new
                    EditCandidateRequestValidator(baseResponse.Warnings, _repoService, _userProvider)
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

                var response = await _candidateService.EditCandidate(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(EditCandidate));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(EditCandidate), ex.Message);
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
