using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using HireHub.Shared.Authentication.Interface;
using HireHub.Shared.Common;
using HireHub.Shared.Common.Exceptions;
using HireHub.Shared.Infrastructure.Interface;
using HireHub.Shared.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HireHub.Core.Service;

public class TokenService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISaveRepository _saveRepository;
    private readonly IAzureEmailService _azureEmailService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OtpService _otpService;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IUserRepository userRepository, IRoleRepository roleRepository, 
        IJwtTokenService jwtTokenService, ISaveRepository saveRepository,
        IAzureEmailService azureEmailService, IHttpClientFactory httpClientFactory,
        OtpService otpService, ILogger<TokenService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtTokenService = jwtTokenService;
        _saveRepository = saveRepository;
        _azureEmailService = azureEmailService;
        _httpClientFactory = httpClientFactory;
        _otpService = otpService;
        _logger = logger;
    }

    public async Task<LoginResponse> GenerateToken(LoginRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GenerateToken));

        var user = await _userRepository.GetByEmailAsync(request.Username);

        if (user == null)
        {
            _logger.LogWarning(LogMessage.UserNotFoundOnLogin, request.Username);
            throw new CommonException(CommonRS.Auth_InvalidCredentials_Format(request.Username));
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return new() { Warnings = [ResponseMessage.PasswordSetRequire] };
        }

        if (!VerifyPassword(user, user.PasswordHash, request.Password))
        {
            _logger.LogWarning(LogMessage.InvalidPassword, user.UserId);
            throw new CommonException(CommonRS.Auth_InvalidCredentials_Format(request.Username));
        }

        if (!user.IsActive)
        {
            _logger.LogWarning(LogMessage.NotActiveUser, user.UserId);
            return new() { Warnings = [ResponseMessage.NotActiveUser] };
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId);

        var token = _jwtTokenService.GenerateToken(user.UserId.ToString(), role!.RoleName.ToString());

        _logger.LogInformation(LogMessage.EndMethod, nameof(GenerateToken));

        if (request.Password.Equals("Welcome@123"))
            return new() {
                Data = token,
                Warnings = [ResponseMessage.PasswordReSetRequire] 
            };

        return new() { Data = token };
    }

    private bool VerifyPassword(User user, string storedPasswordHash, string providedPassword)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(VerifyPassword));

        var hasher = new PasswordHasher<User>();
        // To verify:
        var result = hasher.VerifyHashedPassword(user, storedPasswordHash, providedPassword);

        _logger.LogInformation(LogMessage.EndMethod, nameof(VerifyPassword));

        return result == PasswordVerificationResult.Success;
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(ForgotPasswordAsync));

        if (!_otpService.ValidateOtp($"{Otp.Prefix}{request.Email}", request.Otp))
        {
            _logger.LogWarning(LogMessage.OtpValidationFailed, request.Email);
            throw new CommonException(ResponseMessage.OtpValidationFailed);
        }

        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning(LogMessage.UserNotFound, request.Email);
            throw new CommonException(ResponseMessage.EmailNotFound);
        }

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        _userRepository.Update(user);
        _saveRepository.SaveChanges();

        _logger.LogInformation(LogMessage.EndMethod, nameof(ForgotPasswordAsync));

        return new() { Data = ResponseMessage.UpdatedSuccessfully };
    }

    public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(ChangePasswordAsync));

        var user = await _userRepository.GetByEmailAsync(request.Email!);

        if (user == null)
        {
            _logger.LogWarning(LogMessage.UserNotFound, request.Email);
            throw new CommonException(ResponseMessage.EmailNotFound);
        }

        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return new() { Warnings = [ResponseMessage.PasswordSetRequire] };
        }

        if (!VerifyPassword(user, user.PasswordHash, request.OldPassword))
        {
            _logger.LogWarning(LogMessage.InvalidPassword, user.UserId);
            throw new CommonException(CommonRS.Auth_InvalidCredentials_Format(request.Email));
        }

        if (!user.IsActive)
        {
            _logger.LogWarning(LogMessage.NotActiveUser, user.UserId);
            return new() { Warnings = [ResponseMessage.NotActiveUser] };
        }

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);

        _userRepository.Update(user);
        _saveRepository.SaveChanges();

        _logger.LogInformation(LogMessage.EndMethod, nameof(ChangePasswordAsync));

        return new() { Data = ResponseMessage.PasswordChangedSuccessfully };
    }

    public async Task<VerifyEmailResponse> CheckEmailExistsAync(VerifyEmailRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(CheckEmailExistsAync));

        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning(LogMessage.UserNotFound, request.Email);
            throw new CommonException(ResponseMessage.EmailNotFound);
        }

        var otp = new Random().Next(100000, 999999).ToString();

        var key = $"{Otp.Prefix}{user.Email}";
        _otpService.StoreOtp(key, otp);

        await _azureEmailService.SendEmailAsync(new Email 
            { 
                To = user.Email, 
                Subject = EmailSubject.ForgotPasswordOTP, 
                Body = string.Format(EmailBody.ForgotPasswordOTP, user.FullName, otp)
            }, _httpClientFactory);

        _logger.LogInformation(LogMessage.EndMethod, nameof(CheckEmailExistsAync));

        return new() { Data = otp };
    }
}