using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using HireHub.Shared.Common.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Service;

public class DriveService
{
    private readonly IDriveRepository _driveRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRoundRepository _roundRepository;
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly ISaveRepository _saveRepository;
    private readonly ILogger<DriveService> _logger;

    public DriveService(IDriveRepository driveRepository, IRoleRepository roleRepository,
        IUserRepository userRepository, ICandidateRepository candidateRepository,
        IRoundRepository roundRepository, IFeedbackRepository feedbackRepository,
        ISaveRepository saveRepository, ILogger<DriveService> logger)
    {
        _driveRepository = driveRepository;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _candidateRepository = candidateRepository;
        _roundRepository = roundRepository;
        _feedbackRepository = feedbackRepository;
        _saveRepository = saveRepository;
        _logger = logger;
    }


    #region Query Services

    public async Task<Response<List<DriveDTO>>> GetDrives(DriveStatus? status,
        string? creatorEmail, int? technicalRounds, bool? isLatestFirst, bool includePastDrives, 
        DateTime? startDate, DateTime? endDate, int? pageNumber, int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDrives));

        var filter = new DriveFilter
        {
            Status = status,
            CreatorEmail = creatorEmail,
            TechnicalRounds = technicalRounds,
            IsLatestFirst = isLatestFirst,
            IncludePastDrives = includePastDrives,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var drives = await _driveRepository.GetAllAsync(filter, CancellationToken.None);

        var driveDTOs = ConverToDTO(drives);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetDrives));

        return new()
        {
            Data = driveDTOs
        };
    }


    public async Task<Response<DriveDTO>> GetDrive(int driveId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDrive));

        var drive = await _driveRepository.GetByIdAsync(driveId) ??
            throw new CommonException(ResponseMessage.DriveNotFound);

        var driveDTO = Helper.Map<Drive, DriveDTO>(drive);
        driveDTO.DriveStatus = drive.Status.ToString();
        driveDTO.CreatorName = (await _userRepository.GetByIdAsync(drive.CreatedBy))!.FullName;

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetDrive));

        return new() { Data = driveDTO };
    }


    public async Task<Response<DriveConfigDTO>> GetDriveConfig(int driveId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDriveConfig));

        var driveWithConfig = await _driveRepository.GetDriveWithConfigAsync(driveId) ??
            throw new CommonException(ResponseMessage.DriveNotFound);

        var roles = await _roleRepository.GetAllAsync(CancellationToken.None);

        var driveConfigDTO = ConverToDriveConfigDTO(driveWithConfig, roles.ToList());

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetDriveConfig));

        return new() { Data = driveConfigDTO };
    }


    public async Task<Response<List<DriveMemberDTO>>> GetDriveMembers(int? driveId, int? userId, UserRole? role,
        DriveStatus? driveStatus, bool? isLatestFirst, bool includePastDrives, DateTime? startDate, DateTime? endDate, 
        int? pageNumber, int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDriveMembers));

        var filter = new DriveMemberFilter
        {
            DriveId = driveId,
            UserId = userId,
            Role = role,
            DriveStatus = driveStatus,
            IsLatestFirst = isLatestFirst,
            IncludePastDrives = includePastDrives,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var driveMemberDTOs = await _driveRepository.GetDriveMembersAsDtoAsync(filter, CancellationToken.None);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetDriveMembers));

        return new()
        {
            Data = driveMemberDTOs
        };
    }


    public async Task<Response<List<DriveCandidateDTO>>> GetDriveCandidates(int? driveId, int? candidateId, CandidateStatus? candidateStatus,
        DriveStatus? driveStatus, bool? isLatestFirst, bool includePastDrives, DateTime? startDate, DateTime? endDate,
        int? pageNumber, int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetDriveCandidates));

        var filter = new DriveCandidateFilter
        {
            DriveId = driveId,
            CandidateId = candidateId,
            CandidateStatus = candidateStatus,
            DriveStatus = driveStatus,
            IsLatestFirst = isLatestFirst,
            IncludePastDrives = includePastDrives,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var driveCandidateDTOs = await _driveRepository.GetDriveCandidatesAsDtoAsync(filter, CancellationToken.None);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetDriveCandidates));

        return new()
        {
            Data = driveCandidateDTOs
        };
    }


    public async Task<Response<List<RoundDTO>>> GetInterviewRounds(int? driveId, int? userId, RoundType? roundType,
        RoundStatus? roundStatus, RoundResult? roundResult, int? pageNumber, int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetInterviewRounds));

        var filter = new RoundFilter
        {
            DriveId = driveId,
            UserId = userId,
            RoundType = roundType,
            RoundStatus = roundStatus,
            RoundResult = roundResult,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var roundDTOs = await _roundRepository.GetAllAsDtoAsync(filter, CancellationToken.None);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetInterviewRounds));

        return new()
        {
            Data = roundDTOs
        };
    }


    public async Task<Response<RoundDTO>> GetInterviewRound(int interviewRoundId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetInterviewRound));

        var roundDTO = await _roundRepository.GetByIdAsDtoAsync(interviewRoundId) ??
            throw new CommonException(ResponseMessage.InterviewRoundNotFound);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetInterviewRound));

        return new() { Data = roundDTO };
    }


    public async Task<Response<FeedbackDTO>> GetFeedback(int feedbackId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetFeedback));

        var feedback = await _feedbackRepository.GetByIdAsync(feedbackId) ??
            throw new CommonException(ResponseMessage.FeedbackNotFound);

        var feedbackDTO = Helper.Map<Feedback, FeedbackDTO>(feedback);
        feedbackDTO.CandidateRecommendation = feedback.Recommendation.ToString();

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetFeedback));

        return new() { Data = feedbackDTO };
    }

    #endregion

    #region Command Services

    public async Task<Response<DriveDTO>> CreateDriveAsync(CreateDriveRequest request, int requestUserId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(CreateDriveAsync));

        var drive = new Drive
        {
            DriveName = request.DriveName,
            DriveDate = request.DriveDate,
            TechnicalRounds = request.TechnicalRounds,
            Status = DriveStatus.InProposal,
            CreatedBy = requestUserId,
            CreatedDate = DateTime.Now
        };

        // -------------------------------
        // Drive Members
        // -------------------------------
        var hrRole = await _roleRepository.GetByName(UserRole.HR);
        request.CoordinationTeam.Hrs.ForEach( hrId =>
            drive.DriveMembers.Add( new()
            {
                UserId = hrId,
                RoleId = hrRole.RoleId
            })
        );
        var mentorRole = await _roleRepository.GetByName(UserRole.Mentor);
        request.CoordinationTeam.Mentors.ForEach( mentorId =>
            drive.DriveMembers.Add( new()
            {
                UserId = mentorId,
                RoleId = mentorRole.RoleId
            })
        );
        var panelRole = await _roleRepository.GetByName(UserRole.Panel);
        request.CoordinationTeam.PanelMembers.ForEach(panelmemberId =>
            drive.DriveMembers.Add( new()
            {
                UserId = panelmemberId,
                RoleId = panelRole.RoleId
            })
        );

        // -------------------------------
        // Role Configurations
        // -------------------------------
        var hrRoleConfig = Helper.Map<HrConfigRequest, DriveRoleConfiguration>(request.HrConfiguration);
        hrRoleConfig.RoleId = hrRole.RoleId;
        hrRoleConfig.CanViewFeedback = true;
        drive.DriveRoleConfigurations.Add(hrRoleConfig);

        var panelRoleConfig = Helper.Map<PanelConfigRequest, DriveRoleConfiguration>(request.PanelConfiguration);
        panelRoleConfig.RoleId = panelRole.RoleId;
        panelRoleConfig.AllowBulkUpload = false;
        panelRoleConfig.CanViewFeedback = true;
        drive.DriveRoleConfigurations.Add(panelRoleConfig);

        var mentorRoleConfig = Helper.Map<MentorConfigRequest, DriveRoleConfiguration>(request.MentorConfiguration);
        mentorRoleConfig.RoleId = mentorRole.RoleId;
        mentorRoleConfig.AllowBulkUpload = false;
        mentorRoleConfig.CanEditSubmittedFeedback = false;
        drive.DriveRoleConfigurations.Add(mentorRoleConfig);

        // -------------------------------
        // Panel Visibility
        // -------------------------------
        drive.PanelVisibilitySettings = Helper
            .Map<PanelVisibilityConfigRequest, PanelVisibilitySettings>(request.PanelVisibilityConfiguration);

        // -------------------------------
        // Notification
        // -------------------------------
        drive.NotificationSettings = Helper
            .Map<NotificationConfigRequest, NotificationSettings>(request.NotificationConfiguration);

        // -------------------------------
        // Feedback Configuration
        // -------------------------------
        drive.FeedbackConfiguration = Helper
            .Map<FeedbackConfigRequest, FeedbackConfiguration>(request.FeedbackSettings);

        await _driveRepository.AddAsync(drive, CancellationToken.None);
        _saveRepository.SaveChanges();

        _logger.LogInformation(LogMessage.EndMethod, nameof(CreateDriveAsync));

        return new() { Data = Helper.Map<Drive, DriveDTO>(drive) };
    }


    public async Task<Response<List<int>>> AddCandidatesToDriveAsync(AddCandidatesToDriveRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddCandidatesToDriveAsync));

        var drive = await _driveRepository.GetByIdAsync(request.DriveId) ?? 
                    throw new CommonException(ResponseMessage.DriveNotFound);

        var driveCandidates = new List<DriveCandidate>();

        foreach (int candidateId in request.CandidateIds)
        {
            var candidate = await _candidateRepository.GetByIdAsync(candidateId) ??
                    throw new CommonException(ResponseMessage.CandidateNotFound);

            var driveCandidate = new DriveCandidate
            {
                DriveId = request.DriveId,
                CandidateId = candidateId,
                Status = CandidateStatus.Pending,
                StatusSetBy = null,
                CreatedDate = DateTime.Now
            };
            drive.DriveCandidates.Add(driveCandidate);

            driveCandidates.Add(driveCandidate);
        }

        _driveRepository.Update(drive);
        _saveRepository.SaveChanges();

        _logger.LogInformation(LogMessage.EndMethod, nameof(AddCandidatesToDriveAsync));

        return new() { Data = driveCandidates.Select(e => e.DriveCandidateId).ToList() };
    }


    public async Task<Response<DriveMemberDTO>> AddMemberToDriveAsync(AddMemberToDriveRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddCandidatesToDriveAsync));

        var drive = await _driveRepository.GetDriveWithMembersAsync(request.DriveId) ??
                    throw new CommonException(ResponseMessage.DriveNotFound);

        var isAlreadyAdded = drive.DriveMembers.Any(e => e.UserId == request.MemberId);
        if (isAlreadyAdded) throw new CommonException(ResponseMessage.AlreadyMemberOfDrive);

        var user = await _userRepository.GetByIdAsync(request.MemberId) ??
                    throw new CommonException(ResponseMessage.UserNotFound);

        var driveMember = new DriveMember
        {
            DriveId = request.DriveId,
            UserId = request.MemberId,
            RoleId = user.RoleId
        };
        drive.DriveMembers.Add(driveMember);

        _driveRepository.Update(drive);
        _saveRepository.SaveChanges();

        var driveMemberDTO = Helper.Map<DriveMember, DriveMemberDTO>(driveMember);
        driveMemberDTO.RoleName = (await _roleRepository.GetByIdAsync(user.RoleId))!.RoleName.ToString();
        driveMemberDTO.DriveName = drive.DriveName;
        driveMemberDTO.DriveDate = drive.DriveDate;
        driveMemberDTO.DriveStatus = drive.Status.ToString();
        driveMemberDTO.UserName = user.FullName;
        driveMemberDTO.UserEmail = user.Email;

        _logger.LogInformation(LogMessage.EndMethod, nameof(AddCandidatesToDriveAsync));

        return new() { Data = driveMemberDTO };
    }


    public async Task<Response<DriveDTO>> EditDrive(JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditDrive));

        int driveId = request[JOPropertyName.DriveId]!.ToObject<int>();

        var drive = await _driveRepository.GetByIdAsync(driveId) ??
            throw new CommonException(ResponseMessage.DriveNotFound);

        // ✅ Allowed updates only
        if (request.ContainsKey(JOPropertyName.DriveName))
            drive.DriveName = request[JOPropertyName.DriveName]!.ToString();

        if (request.ContainsKey(JOPropertyName.DriveDate))
            drive.DriveDate = request[JOPropertyName.DriveDate]!.ToObject<DateTime>();

        if (request.ContainsKey(JOPropertyName.TechnicalRounds))
            drive.TechnicalRounds = request[JOPropertyName.TechnicalRounds]!.ToObject<int>();

        if (request.ContainsKey(JOPropertyName.DriveStatus))
            drive.Status = (DriveStatus)Enum.Parse(typeof(DriveStatus), request[JOPropertyName.DriveStatus]!.ToString());

        // ❌ NOT updating CreatedBy & CreatedDate

        _driveRepository.Update(drive);
        _saveRepository.SaveChanges();

        var driveDTO = Helper.Map<Drive, DriveDTO>(drive);
        driveDTO.DriveStatus = drive.Status.ToString();
        driveDTO.CreatorName = (await _userRepository.GetByIdAsync(drive.CreatedBy))!.FullName;

        _logger.LogInformation(LogMessage.EndMethod, nameof(EditDrive));

        return new() { Data = driveDTO };
    }


    public async Task<Response<DriveConfigDTO>> EditDriveConfig(JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditDriveConfig));

        int driveId = request[JOPropertyName.DriveId]!.ToObject<int>();

        var drive = await _driveRepository.GetDriveWithConfigAsync(driveId) ??
            throw new CommonException(ResponseMessage.DriveNotFound);

        // ✅ Allowed updates only
        if (request.ContainsKey(JOPropertyName.PanelVisibilitySettings)) 
        {
            var token = request.SelectToken(JOPropertyName.PVS_ShowPhone);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowPhone = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PVS_ShowEmail);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowEmail = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PVS_ShowPreviousCompany);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowPreviousCompany = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PVS_ShowResume);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowResume = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PVS_ShowCollege);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowCollege = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PVS_ShowAddress);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowAddress = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PVS_ShowLinkedIn);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowLinkedIn = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PVS_ShowGitHub);
            if (token != null)
                drive.PanelVisibilitySettings!.ShowGitHub = token.ToObject<bool>();

        }

        if (request.ContainsKey(JOPropertyName.NotificationSettings))
        {
            var token = request.SelectToken(JOPropertyName.NS_EmailNotificationEnabled);
            if (token != null)
                drive.NotificationSettings!.EmailNotificationEnabled = token.ToObject<bool>();
        }

        if (request.ContainsKey(JOPropertyName.FeedbackConfiguration))
        {
            var token = request.SelectToken(JOPropertyName.FC_OverallRatingRequired);
            if (token != null)
                drive.FeedbackConfiguration!.OverallRatingRequired = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.FC_TechnicalSkillRequired);
            if (token != null)
                drive.FeedbackConfiguration!.TechnicalSkillRequired = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.FC_CommunicationRequired);
            if (token != null)
                drive.FeedbackConfiguration!.CommunicationRequired = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.FC_ProblemSolvingRequired);
            if (token != null)
                drive.FeedbackConfiguration!.ProblemSolvingRequired = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.FC_RecommendationRequired);
            if (token != null)
                drive.FeedbackConfiguration!.RecommendationRequired = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.FC_OverallFeedbackRequired);
            if (token != null)
                drive.FeedbackConfiguration!.OverallFeedbackRequired = token.ToObject<bool>();
        }

        var roles = await _roleRepository.GetAllAsync(CancellationToken.None);

        var hrRole = roles.First(e => e.RoleName == UserRole.HR);
        var panelRole = roles.First(e => e.RoleName == UserRole.Panel);
        var mentorRole = roles.First(e => e.RoleName == UserRole.Mentor);

        var hrConfig = drive.DriveRoleConfigurations.First(e => e.RoleId == hrRole.RoleId);
        if (request.ContainsKey(JOPropertyName.HrConfiguration))
        {
            var token = request.SelectToken(JOPropertyName.HC_AllowBulkUpload);
            if (token != null)
                hrConfig.AllowBulkUpload = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.HC_CanEditSubmittedFeedback);
            if (token != null)
                hrConfig.CanEditSubmittedFeedback = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.HC_AllowPanelReassign);
            if (token != null)
                hrConfig.AllowPanelReassign = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.HC_RequireApprovalForReassignment);
            if (token != null)
                hrConfig.RequireApprovalForReassignment = token.ToObject<bool>();
        }

        var panelConfig = drive.DriveRoleConfigurations.First(e => e.RoleId == panelRole.RoleId);
        if (request.ContainsKey(JOPropertyName.PanelConfiguration))
        {
            var token = request.SelectToken(JOPropertyName.PC_CanEditSubmittedFeedback);
            if (token != null)
                panelConfig.CanEditSubmittedFeedback = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PC_AllowPanelReassign);
            if (token != null)
                panelConfig.AllowPanelReassign = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.PC_RequireApprovalForReassignment);
            if (token != null)
                panelConfig.RequireApprovalForReassignment = token.ToObject<bool>();
        }

        var mentorConfig = drive.DriveRoleConfigurations.First(e => e.RoleId == mentorRole.RoleId);
        if (request.ContainsKey(JOPropertyName.MentorConfiguration))
        {
            var token = request.SelectToken(JOPropertyName.MC_CanViewFeedback);
            if (token != null)
                mentorConfig.CanViewFeedback = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.MC_AllowPanelReassign);
            if (token != null)
                mentorConfig.AllowPanelReassign = token.ToObject<bool>();
            token = request.SelectToken(JOPropertyName.MC_RequireApprovalForReassignment);
            if (token != null)
                mentorConfig.RequireApprovalForReassignment = token.ToObject<bool>();
        }

        _driveRepository.Update(drive);
        _saveRepository.SaveChanges();

        var driveConfigDTO = ConverToDriveConfigDTO(drive, roles.ToList());

        _logger.LogInformation(LogMessage.EndMethod, nameof(EditDriveConfig));

        return new() { Data = driveConfigDTO };
    }


    public async Task<Response<DriveMemberDTO>> RemoveDriveMember(RemoveDriveMemberRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(RemoveDriveMember));

        var drive = await _driveRepository.GetDriveWithMembersAsync(request.DriveId) ??
            throw new CommonException(ResponseMessage.DriveNotFound);

        var driveMember = drive.DriveMembers.FirstOrDefault(e => e.UserId == request.MemberId) ??
            throw new CommonException(ResponseMessage.DriveMemberNotFound);

        _driveRepository.RemoveDriveMember(driveMember);
        _saveRepository.SaveChanges();

        var user = await _userRepository.GetByIdAsync(request.MemberId) ??
                    throw new CommonException(ResponseMessage.UserNotFound);

        var driveMemberDTO = Helper.Map<DriveMember, DriveMemberDTO>(driveMember);
        driveMemberDTO.RoleName = (await _roleRepository.GetByIdAsync(driveMember.RoleId))!.RoleName.ToString();
        driveMemberDTO.DriveName = drive.DriveName;
        driveMemberDTO.DriveDate = drive.DriveDate;
        driveMemberDTO.DriveStatus = drive.Status.ToString();
        driveMemberDTO.UserName = user.FullName;
        driveMemberDTO.UserEmail = user.Email;

        _logger.LogInformation(LogMessage.EndMethod, nameof(RemoveDriveMember));

        return new() { Data = driveMemberDTO };
    }


    public async Task<Response<List<int>>> RemoveCandidatesFromDrive(RemoveDriveCandidatesRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(RemoveCandidatesFromDrive));

        var drive = await _driveRepository.GetDriveWithCandidatesAsync(request.DriveId, request.CandidateIds) ??
            throw new CommonException(ResponseMessage.DriveNotFound);

        var driveCandidateIds = new List<int>();
        foreach (var candidateId in request.CandidateIds)
        {
            var driveCandidate = drive.DriveCandidates.FirstOrDefault(e => e.CandidateId == candidateId) ??
                throw new CommonException(ResponseMessage.DriveCandidateNotFound + $" : {candidateId}");
            _driveRepository.RemoveDriveCandidate(driveCandidate);
            driveCandidateIds.Add(driveCandidate.DriveCandidateId);
        }

        _saveRepository.SaveChanges();

        _logger.LogInformation(LogMessage.EndMethod, nameof(RemoveCandidatesFromDrive));

        return new() { Data = driveCandidateIds };
    }


    public async Task<Response<DriveCandidateDTO>> EditDriveCandidate(JObject request, int requestUserId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditDriveCandidate));

        int driveId = request[JOPropertyName.DriveId]!.ToObject<int>();
        int candidateId = request[JOPropertyName.CandidateId]!.ToObject<int>();

        var drive = await _driveRepository.GetDriveWithCandidatesAsync(driveId, [candidateId]) ??
            throw new CommonException(ResponseMessage.DriveNotFound);

        var driveCandidate = drive.DriveCandidates.FirstOrDefault(e => e.CandidateId == candidateId) ??
            throw new CommonException(ResponseMessage.DriveCandidateNotFound);

        if (request.ContainsKey(JOPropertyName.CandidateStatus))
        {
            driveCandidate.Status = Enum
                .Parse<CandidateStatus>(request[JOPropertyName.CandidateStatus]!.ToString());
            driveCandidate.StatusSetBy = requestUserId;
        }

        _driveRepository.Update(drive);
        _saveRepository.SaveChanges();

        var candidate = await _candidateRepository.GetByIdAsync(candidateId) ??
                    throw new CommonException(ResponseMessage.CandidateNotFound);

        var driveCandidateDTO = Helper.Map<DriveCandidate, DriveCandidateDTO>(driveCandidate);
        driveCandidateDTO.CandidateStatus = driveCandidate.Status.ToString();
        driveCandidateDTO.DriveName = drive.DriveName;
        driveCandidateDTO.DriveDate = drive.DriveDate;
        driveCandidateDTO.DriveStatus = drive.Status.ToString();
        driveCandidateDTO.CandidateName = candidate.FullName;
        driveCandidateDTO.CandidateEmail = candidate.Email;

        _logger.LogInformation(LogMessage.EndMethod, nameof(EditDriveCandidate));

        return new() { Data = driveCandidateDTO };
    }


    public async Task<Response<RoundDTO>> EditInterviewRound(JObject request, int requestUserId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditInterviewRound));

        int roundId = request[JOPropertyName.RoundId]!.ToObject<int>();
        var round = await _roundRepository.GetRoundByIdWithDetails(roundId) ??
            throw new CommonException(ResponseMessage.InterviewRoundNotFound);

        if (request.ContainsKey(JOPropertyName.RoundStatus))
            round.Status = Enum
                .Parse<RoundStatus>(request[JOPropertyName.RoundStatus]!.ToString());

        if (request.ContainsKey(JOPropertyName.RoundResult))
        {
            round.Result = Enum
                .Parse<RoundResult>(request[JOPropertyName.RoundResult]!.ToString());
            UpdateDriveCandidateStatusBasedOnRoundResult(round, requestUserId);
        }

        _roundRepository.Update(round);
        _saveRepository.SaveChanges();

        var roundDTO = await _roundRepository.GetByIdAsDtoAsync(roundId);

        _logger.LogInformation(LogMessage.EndMethod, nameof(EditInterviewRound));

        return new() { Data = roundDTO };
    }


    public async Task<Response<FeedbackDTO>> EditFeedback(JObject request, int requestUserId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditFeedback));

        int roundId = request[JOPropertyName.RoundId]!.ToObject<int>();
        var round = await _roundRepository.GetRoundByIdWithDetails(roundId) ??
            throw new CommonException(ResponseMessage.InterviewRoundNotFound);

        if (round.FeedbackId == null) 
            throw new CommonException(ResponseMessage.NoFeedbackProvidedForRound);

        var feedback = await _feedbackRepository.GetByIdAsync(round.FeedbackId) ??
            throw new CommonException(ResponseMessage.FeedbackNotFound);

        if (request.ContainsKey(JOPropertyName.OverallRating))
            feedback.OverallRating = request[JOPropertyName.OverallRating]!.ToObject<int?>();
        if (request.ContainsKey(JOPropertyName.TechnicalSkill))
            feedback.TechnicalSkill = request[JOPropertyName.TechnicalSkill]!.ToObject<int?>();
        if (request.ContainsKey(JOPropertyName.Communication))
            feedback.Communication = request[JOPropertyName.Communication]!.ToObject<int?>();
        if (request.ContainsKey(JOPropertyName.ProblemSolving))
            feedback.ProblemSolving = request[JOPropertyName.ProblemSolving]!.ToObject<int?>();
        if (request.ContainsKey(JOPropertyName.OverallFeedback))
            feedback.OverallFeedback = request[JOPropertyName.OverallFeedback]!.ToString();
        if (request.ContainsKey(JOPropertyName.Recommendation))
        {
            feedback.Recommendation = Enum
                .Parse<Recommendation>(request[JOPropertyName.Recommendation]!.ToString());
            round.Result = feedback.Recommendation switch
            {
                Recommendation.Hire => RoundResult.Selected,
                Recommendation.Maybe => RoundResult.Selected,
                Recommendation.NoHire => RoundResult.Rejected,
                _ => RoundResult.Pending
            };
            UpdateDriveCandidateStatusBasedOnRoundResult(round, requestUserId);
        }
        feedback.SubmittedDate = DateTime.Now;

        _feedbackRepository.Update(feedback);
        _roundRepository.Update(round);
        _saveRepository.SaveChanges();

        var feedbackDTO = Helper.Map<Feedback, FeedbackDTO>(feedback);
        feedbackDTO.CandidateRecommendation = feedback.Recommendation.ToString();

        _logger.LogInformation(LogMessage.EndMethod, nameof(EditFeedback));

        return new() { Data = feedbackDTO };
    }


    public async Task<Response<FeedbackDTO>> AddFeedback(AddFeedbackRequest request, int requestUserId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddFeedback));

        var round = await _roundRepository.GetRoundByIdWithDetails(request.RoundId) ??
            throw new CommonException(ResponseMessage.InterviewRoundNotFound);

        if (round.FeedbackId != null)
            throw new CommonException(ResponseMessage.FeedbackAlreadyProvided);

        var feedback = Helper.Map<AddFeedbackRequest, Feedback>(request);
        feedback.Recommendation = Enum.Parse<Recommendation>(request.CandidateRecommendation);

        round.Feedback = feedback;

        round.Result = feedback.Recommendation switch
        {
            Recommendation.Hire => RoundResult.Selected,
            Recommendation.Maybe => RoundResult.Selected,
            Recommendation.NoHire => RoundResult.Rejected,
            _ => RoundResult.Pending
        };
        UpdateDriveCandidateStatusBasedOnRoundResult(round, requestUserId);

        _roundRepository.Update(round);
        _saveRepository.SaveChanges();

        var feedbackDTO = Helper.Map<Feedback, FeedbackDTO>(feedback);
        feedbackDTO.CandidateRecommendation = feedback.Recommendation.ToString();

        _logger.LogInformation(LogMessage.EndMethod, nameof(AddFeedback));

        return new() { Data = feedbackDTO };
    }

    #endregion

    #region Private Methods

    private List<DriveDTO> ConverToDTO(List<Drive> drives)
    {
        var driveDTOs = new List<DriveDTO>();
        drives.ForEach(drive =>
        {
            var driveDTO = Helper.Map<Drive, DriveDTO>(drive);
            driveDTO.CreatorName = drive.Creator!.FullName;
            driveDTO.DriveStatus = drive.Status.ToString();
            driveDTOs.Add(driveDTO);
        });
        return driveDTOs;
    }

    private DriveConfigDTO ConverToDriveConfigDTO(Drive driveWithConfig, List<Role> roles)
    {
        var hrRole = roles.First(e => e.RoleName == UserRole.HR);
        var panelRole = roles.First(e => e.RoleName == UserRole.Panel);
        var mentorRole = roles.First(e => e.RoleName == UserRole.Mentor);

        var hrConfig = driveWithConfig.DriveRoleConfigurations.First(e => e.RoleId == hrRole.RoleId);
        var panelConfig = driveWithConfig.DriveRoleConfigurations.First(e => e.RoleId == panelRole.RoleId);
        var mentorConfig = driveWithConfig.DriveRoleConfigurations.First(e => e.RoleId == mentorRole.RoleId);

        return new DriveConfigDTO
        {
            DriveId = driveWithConfig.DriveId,
            HrConfiguration = Helper.Map<DriveRoleConfiguration, HrConfigurationDTO>(hrConfig),
            PanelConfiguration = Helper.Map<DriveRoleConfiguration, PanelConfigurationDTO>(hrConfig),
            MentorConfiguration = Helper.Map<DriveRoleConfiguration, MentorConfigurationDTO>(hrConfig),
            PanelVisibilitySettings = Helper.Map<PanelVisibilitySettings, PanelVisibilitySettingsDTO>(driveWithConfig.PanelVisibilitySettings!),
            NotificationSettings = Helper.Map<NotificationSettings, NotificationSettingsDTO>(driveWithConfig.NotificationSettings!),
            FeedbackConfiguration = Helper.Map<FeedbackConfiguration, FeedbackConfigurationDTO>(driveWithConfig.FeedbackConfiguration!)
        };
    }

    private List<DriveMemberDTO> ConverToDriveMemberDTO(List<DriveMember> driveMembers)
    {
        var driveMemberDTOs = new List<DriveMemberDTO>();
        driveMembers.ForEach(driveMember =>
        {
            var driveMemberDTO = Helper.Map<DriveMember, DriveMemberDTO>(driveMember);
            driveMemberDTO.DriveName = driveMember.Drive!.DriveName;
            driveMemberDTO.DriveDate = driveMember.Drive.DriveDate;
            driveMemberDTO.DriveStatus = driveMember.Drive.Status.ToString();
            driveMemberDTO.UserName = driveMember.User!.FullName;
            driveMemberDTO.UserEmail = driveMember.User.Email;
            driveMemberDTO.RoleName = driveMember.Role!.RoleName.ToString();
            driveMemberDTOs.Add(driveMemberDTO);
        });
        return driveMemberDTOs;
    }

    private List<DriveCandidateDTO> ConverToDriveCandidateDTO(List<DriveCandidate> driveCandidates)
    {
        var driveCandidateDTOs = new List<DriveCandidateDTO>();
        driveCandidates.ForEach(driveCandidate =>
        {
            var driveCandidateDTO = Helper.Map<DriveCandidate, DriveCandidateDTO>(driveCandidate);
            driveCandidateDTO.DriveName = driveCandidate.Drive!.DriveName;
            driveCandidateDTO.DriveDate = driveCandidate.Drive.DriveDate;
            driveCandidateDTO.DriveStatus = driveCandidate.Drive.Status.ToString();
            driveCandidateDTO.CandidateName = driveCandidate.Candidate!.FullName;
            driveCandidateDTO.CandidateEmail = driveCandidate.Candidate.Email;
            driveCandidateDTO.CandidateStatus = driveCandidate.Status.ToString();
            driveCandidateDTOs.Add(driveCandidateDTO);
        });
        return driveCandidateDTOs;
    }

    private void UpdateDriveCandidateStatusBasedOnRoundResult(Round round, int requestUserId)
    {
        if (round.RoundType == RoundType.Hr)
        {
            round.DriveCandidate!.Status = round.Result switch
            {
                RoundResult.Selected => CandidateStatus.Selected,
                RoundResult.Rejected => CandidateStatus.Rejected,
                _ => CandidateStatus.Pending
            };
            round.DriveCandidate.StatusSetBy = requestUserId;
        }
    }

    #endregion
}
