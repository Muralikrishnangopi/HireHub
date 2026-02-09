using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using HireHub.Shared.Common.Exceptions;
using HireHub.Shared.Infrastructure.Interface;
using HireHub.Shared.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace HireHub.Core.Service;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDriveRepository _driveRepository;
    private readonly IAzureEmailService _azureEmailService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<UserService> _logger;
    private readonly ISaveRepository _saveRepository;

    public UserService(IUserRepository userRepository, IRoleRepository roleRepository,
        IDriveRepository driveRepository,
        IAzureEmailService azureEmailService, IHttpClientFactory httpClientFactory,
        ILogger<UserService> logger, ISaveRepository saveRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _driveRepository = driveRepository;
        _azureEmailService = azureEmailService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _saveRepository = saveRepository;
    }


    #region Query Services

    public async Task<Response<List<UserDTO>>> GetUsers(UserRole? role, bool? isActive,
        bool? isLatestFirst, DateTime? startDate, DateTime? endDate, int? pageNumber, int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetUsers));

        var filter = new UserFilter
        {
            Role = role, IsActive = isActive, IsLatestFirst = isLatestFirst,
            StartDate = startDate, EndDate = endDate,
            PageNumber = pageNumber, PageSize = pageSize
        };
        var users = await _userRepository.GetAllAsync(filter, CancellationToken.None);

        var userDTOs = ConverToDTO(users);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetUsers));

        return new() { Data = userDTOs };
    }


    public async Task<Response<UserDTO>> GetUser(int userId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetUser));

        var user = await _userRepository.GetByIdAsync(userId, CancellationToken.None) ??
            throw new CommonException(ResponseMessage.UserNotFound);

        var userDTO = Helper.Map<User, UserDTO>(user);
        var role = await _roleRepository.GetByIdAsync(user.RoleId, CancellationToken.None);
        userDTO.RoleName = role!.RoleName.ToString();

       _logger.LogInformation(LogMessage.EndMethod, nameof(GetUser));

        return new() { Data = userDTO };
    }

    public async Task<Response<List<DriveWithCandidatesDto>>> GetDrivewithCandidate(int mentorId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDrivewithCandidate));

        var user = await _userRepository.GetByIdAsync(
            mentorId,
            CancellationToken.None
        ) ?? throw new CommonException(ResponseMessage.UserNotFound);

        var drives = await _driveRepository
            .GetMentorDrivesWithCandidatesAsync(mentorId, CancellationToken.None);

        var result = drives.Select(d => new DriveWithCandidatesDto
        {
            DriveId = d.DriveId,
            DriveName = d.DriveName,
            DriveDate = d.DriveDate,

            Candidates = d.DriveCandidates
                .Where(dc => dc.Candidate != null)
                .Select(dc =>
                {
                    var latestRound = dc.Rounds
                        .OrderByDescending(r => r.RoundId)
                        .FirstOrDefault();

                    return new MentorCandidateDto
                    {
                        // Candidate
                        CandidateId = dc.Candidate!.CandidateId,
                        FullName = dc.Candidate.FullName,
                        Email = dc.Candidate.Email,
                        Phone = dc.Candidate.Phone,
                        Address = dc.Candidate.Address,
                        College = dc.Candidate.College,
                        PreviousCompany = dc.Candidate.PreviousCompany,

                        // DriveCandidate
                        AttendanceStatus = dc.Attendance_Status,
                        Status = dc.Status,

                        // Round
                        RoundType = latestRound?.RoundType,
                        RoundStatus = latestRound?.Status,
                        RoundResult = latestRound?.Result,

                        // Interviewer (User)
                        InterviewerId = latestRound?.Interviewer?.UserId,
                        InterviewerName = latestRound?.Interviewer?.User?.FullName,
                        InterviewerEmail = latestRound?.Interviewer?.User?.Email,
                        InterviewerPhone = latestRound?.Interviewer?.User?.Phone
                    };
                })
                .ToList()
        }).ToList();

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetDrivewithCandidate));

        return new Response<List<DriveWithCandidatesDto>>
        {
            Data = result
        };
    }

    #endregion

    #region Command Services

    public async Task<Response<UserDTO>> AddUser(AddUserRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddUser));

        var user = Helper.Map<AddUserRequest, User>(request);

        var role = await _roleRepository
            .GetByName((UserRole)Enum.Parse(typeof(UserRole), request.RoleName, true));
        user.RoleId = role.RoleId;

        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        // Set Default Permissions for User
        var permission = (role.RoleName == UserRole.Admin || role.RoleName == UserRole.HR) ?
            new UserPermission {
                Action = UserAction.Drive,
                View = true, Add = true, Update = true, Delete = role.RoleName == UserRole.Admin
            } :
            new UserPermission {
                Action = UserAction.Drive,
                View = true, Add = false, Update = false, Delete = false
            };
        user.UserPermissions.Add(permission);

        await _userRepository.AddAsync(user, CancellationToken.None);
        _saveRepository.SaveChanges();

        var userDTO = Helper.Map<User, UserDTO>(user);
        userDTO.RoleName = role.RoleName.ToString();

        try {
            await _azureEmailService.SendEmailAsync(new Email {
                To = user.Email,
                Subject = EmailSubject.NewUserWelcome,
                Body = string.Format(EmailBody.NewUserWelcome, user.FullName, user.Email, request.Password)
            }, _httpClientFactory);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains(InnerExceptionMessage.MsgTriggeredToInvalidEmail))
        {
            throw new CommonException(ResponseMessage.InvalidEmail);
        }

        _logger.LogInformation(LogMessage.EndMethod, nameof(AddUser));

        return new() { Data = userDTO };
    }


    public async Task<Response<UserDTO>> EditUser(JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditUser));

        int userId = request[JOPropertyName.UserId]!.ToObject<int>();

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new CommonException(ResponseMessage.UserNotFound);

        var oldEmail = user.Email;

        // ✅ Allowed updates only
        if (request.ContainsKey(JOPropertyName.FullName))
            user.FullName = request[JOPropertyName.FullName]!.ToString();

        if (request.ContainsKey(JOPropertyName.Email))
            user.Email = request[JOPropertyName.Email]!.ToString();

        if (request.ContainsKey(JOPropertyName.Phone))
            user.Phone = request[JOPropertyName.Phone]!.ToString();

        if (request.ContainsKey(JOPropertyName.IsActive))
            user.IsActive = request[JOPropertyName.IsActive]!.ToObject<bool>();

        if (request.ContainsKey(JOPropertyName.RoleName))
        {
            var roleName = (UserRole)Enum.Parse(typeof(UserRole), request[JOPropertyName.RoleName]!.ToString());
            user.RoleId = (await _roleRepository.GetByName(roleName)).RoleId;
        }

        // ❌ NOT updating PasswordHash & CreatedDate
        user.UpdatedDate = DateTime.Now;

        _userRepository.Update(user);
        _saveRepository.SaveChanges();

        var userDTO = Helper.Map<User, UserDTO>(user);
        userDTO.RoleName = (await _roleRepository.GetByIdAsync(user.RoleId))!.RoleName.ToString();

        if (request.ContainsKey(JOPropertyName.Email))
            try {
                await _azureEmailService.SendEmailAsync(new Email {
                    To = user.Email,
                    Subject = EmailSubject.EmailChangedNotification,
                    Body = string.Format(EmailBody.EmailChangedNotification, user.FullName, oldEmail, user.Email)
                }, _httpClientFactory);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains(InnerExceptionMessage.MsgTriggeredToInvalidEmail))
            {
                throw new CommonException(ResponseMessage.InvalidEmail);
            }

        _logger.LogInformation(LogMessage.EndMethod, nameof(EditUser));

        return new() { Data = userDTO };
    }

    #endregion

    #region Private Methods

    private List<UserDTO> ConverToDTO(List<User> users)
    {
        var userDTOs = new List<UserDTO>();
        users.ForEach(user =>
        {
            var userDTO = Helper.Map<User, UserDTO>(user);
            userDTO.RoleName = user.Role!.RoleName.ToString();
            userDTOs.Add(userDTO);
        });
        return userDTOs;
    }

    #endregion

}
