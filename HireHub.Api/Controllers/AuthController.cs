using HireHub.Core.Data.Interface;
using HireHub.Core.DTO;
using HireHub.Core.DTO.Base;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Validators;
using HireHub.Shared.Common.Exceptions;
using HireHub.Shared.Common.Models;
using HireHub.Shared.Middleware.Models;
using Microsoft.AspNetCore.Mvc;

namespace HireHub.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(TokenService tokenService, ITransactionRepository transactionRepository,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }


    [HttpPost("token")]
    [ProducesResponseType<LoginResponse>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> Token([FromBody] LoginRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(Token));

        try
        {
            var baseResponse = new BaseResponse();

            var validator = await new LoginRequestValidator(baseResponse.Warnings).ValidateAsync(request);

            if (!validator.IsValid)
            {
                validator.Errors.ForEach( e =>
                    baseResponse.Errors.Add( new ValidationError { 
                        PropertyName = e.PropertyName, 
                        ErrorMessage = e.ErrorMessage 
                    }) 
                );
                return BadRequest(baseResponse);
            }

            var response = await _tokenService.GenerateToken(request);

            baseResponse.Warnings.ForEach(response.Warnings.Add);

            _logger.LogInformation(LogMessage.EndMethod, nameof(Token));

            return Ok(response);
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(Token), ex.Message);
            return BadRequest( new BaseResponse { 
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ] 
            });
        }
    }


    [HttpPost("password/forgot")]
    [ProducesResponseType<ForgotPasswordResponse>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(ForgotPassword));

        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new ForgotPasswordRequestValidator(baseResponse.Warnings)
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

                var response = await _tokenService.ForgotPasswordAsync(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(ForgotPassword));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(ForgotPassword), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [HttpPost("password/change")]
    [ProducesResponseType<ChangePasswordResponse>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ErrorResponse>(400)]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        try
        {
            using (_transactionRepository.BeginTransaction())
            {
                var baseResponse = new BaseResponse();

                var validator = await new ChangePasswordValidator(baseResponse.Warnings).ValidateAsync(request);

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

                var response = await _tokenService.ChangePasswordAsync(request);

                baseResponse.Warnings.ForEach(response.Warnings.Add);

                _transactionRepository.CommitTransaction();

                _logger.LogInformation(LogMessage.EndMethod, nameof(ChangePassword));

                return Ok(response);
            }
        }
        catch (CommonException ex)
        {
            _logger.LogWarning(LogMessage.EndMethodException, nameof(ChangePassword), ex.Message);
            _transactionRepository.RollbackTransaction();
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }


    [HttpPost("email/verify")]
    [ProducesResponseType<VerifyEmailResponse>(200)]
    [ProducesResponseType<BaseResponse>(400)]
    [ProducesResponseType<ErrorResponse>(500)]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(VerifyEmail));
        try
        {
            var baseResponse = new BaseResponse();

            var validator = await new VerifyEmailRequestValidator(baseResponse.Warnings)
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

            var result = await _tokenService.CheckEmailExistsAync(request);

            baseResponse.Warnings.ForEach(result.Warnings.Add);

            _logger.LogInformation(LogMessage.EndMethod, nameof(VerifyEmail));

            return Ok(result);
        }
        catch (CommonException ex)
        {

            _logger.LogWarning(LogMessage.EndMethodException, nameof(VerifyEmail), ex.Message);
            return BadRequest(new BaseResponse
            {
                Errors = [
                    new ValidationError { PropertyName = PropertyName.Main, ErrorMessage = ex.Message }
                ]
            });
        }
    }

}
