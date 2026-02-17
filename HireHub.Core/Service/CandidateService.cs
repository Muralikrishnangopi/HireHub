using HireHub.Core.Data.Filters;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using HireHub.Shared.Common.Exceptions;
using HireHub.Shared.Common.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Service;

public class CandidateService
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly ISaveRepository _saveRepository;
    private readonly ILogger<CandidateService> _logger;

    public CandidateService(ICandidateRepository candidateRepository,
        ISaveRepository saveRepository, ILogger<CandidateService> logger)
    {
        _candidateRepository = candidateRepository;
        _saveRepository = saveRepository;
        _logger = logger;
    }


    #region Query Services

    public async Task<Response<List<CandidateDTO>>> GetCandidates(CandidateExperienceLevel? experienceLevel,
        bool? isLatestFirst, DateTime? startDate, DateTime? endDate, int? pageNumber, int? pageSize)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetCandidates));

        var filter = new CandidateFilter
        {
            ExperienceLevel = experienceLevel, IsLatestFirst = isLatestFirst,
            StartDate = startDate, EndDate = endDate,
            PageNumber = pageNumber, PageSize = pageSize
        };
        var candidates = await _candidateRepository.GetAllAsync(filter, CancellationToken.None);

        var candidateDTOs = ConverToDTO(candidates);

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetCandidates));

        return new() { Data = candidateDTOs };
    }


    public async Task<Response<CandidateDTO>> GetCandidate(int candidateId)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(GetCandidate));

        var candidate = await _candidateRepository.GetByIdAsync(candidateId) ??
            throw new CommonException(ResponseMessage.CandidateNotFound);

        var candidateDTO = Helper.Map<Candidate, CandidateDTO>(candidate);
        candidateDTO.CandidateExperienceLevel = candidate.ExperienceLevel.ToString();

        _logger.LogInformation(LogMessage.EndMethod, nameof(GetCandidate));

        return new() { Data = candidateDTO };
    }

    #endregion

    #region Command Services

    public async Task<Response<CandidateDTO>> AddCandidate(AddCandidateRequest request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(AddCandidate));

        var candidate = Helper.Map<AddCandidateRequest, Candidate>(request);
        candidate.ExperienceLevel = (CandidateExperienceLevel)Enum
            .Parse(typeof(CandidateExperienceLevel), request.ExperienceLevelName, true);

        await _candidateRepository.AddAsync(candidate, CancellationToken.None);
        _saveRepository.SaveChanges();

        var candidateDTO = Helper.Map<Candidate, CandidateDTO>(candidate);
        candidateDTO.CandidateExperienceLevel = candidate.ExperienceLevel.ToString();

        _logger.LogInformation(LogMessage.EndMethod, nameof(AddCandidate));

        return new() { Data = candidateDTO };
    }

    public async Task<Response<CandidateDTO>> EditCandidate(JObject request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(EditCandidate));

        var response = new BaseResponse();

        int candidateId = request[JOPropertyName.CandidateId]!.ToObject<int>();

        var candidate = await _candidateRepository.GetByIdAsync(candidateId);

        if (candidate == null)
            throw new CommonException(ResponseMessage.CandidateNotFound);

        // ✅ Allowed updates only
        if (request.ContainsKey(JOPropertyName.FullName))
            candidate.FullName = request[JOPropertyName.FullName]!.ToString();

        if (request.ContainsKey(JOPropertyName.Email))
            candidate.Email = request[JOPropertyName.Email]!.ToString();

        if (request.ContainsKey(JOPropertyName.Phone))
            candidate.Phone = request[JOPropertyName.Phone]!.ToString();

        if (request.ContainsKey(JOPropertyName.Address))
            candidate.Address = request[JOPropertyName.Address]?.ToString();

        if (request.ContainsKey(JOPropertyName.College))
            candidate.College = request[JOPropertyName.College]?.ToString();

        if (request.ContainsKey(JOPropertyName.PreviousCompany))
            candidate.PreviousCompany = request[JOPropertyName.PreviousCompany]?.ToString();

        if (request.ContainsKey(JOPropertyName.ExperienceLevel))
        {
            var experienceLevel = Enum.Parse(typeof(CandidateExperienceLevel), request[JOPropertyName.ExperienceLevel]!.ToString());
            candidate.ExperienceLevel = (CandidateExperienceLevel)experienceLevel;
        }

        if (request.ContainsKey(JOPropertyName.TechStack))
            candidate.TechStack = request[JOPropertyName.TechStack]!.ToObject<List<string>>() ?? candidate.TechStack;

        if (request.ContainsKey(JOPropertyName.ResumeUrl))
            candidate.ResumeUrl = request[JOPropertyName.ResumeUrl]?.ToString();

        if (request.ContainsKey(JOPropertyName.LinkedInUrl))
            candidate.LinkedInUrl = request[JOPropertyName.LinkedInUrl]?.ToString();

        if (request.ContainsKey(JOPropertyName.GitHubUrl))
            candidate.GitHubUrl = request[JOPropertyName.GitHubUrl]?.ToString();

        _candidateRepository.Update(candidate);
        _saveRepository.SaveChanges();

        var candidateDTO = Helper.Map<Candidate, CandidateDTO>(candidate);
        candidateDTO.CandidateExperienceLevel = candidate.ExperienceLevel.ToString();

        _logger.LogInformation(LogMessage.EndMethod, nameof(EditCandidate));

        return new() { Data = candidateDTO };
    }

    public async Task<bool> MarkAttendance(
 int driveId,
 int candidateId,
  string Attendance_Status,
 int currentUserId
)
    {
        var driveCandidate =
            await _candidateRepository.GetValidDriveCandidateForAttendance(driveId, candidateId, currentUserId);

        if (driveCandidate == null)
            throw new CommonException(
                "Attendance allowed only for today's active drive.");

        // Business rule
        if (driveCandidate.Attendance_Status == "Present")
            throw new CommonException("Attendance already marked.");

        // Update
        driveCandidate.Attendance_Status = Attendance_Status;
        driveCandidate.StatusSetBy = currentUserId;

        await _saveRepository.SaveChangesAsync(); // or UnitOfWork

        return true;
    }
    public async Task<Response<RequestReassignmentDto>> RequestReassignmentAsync(
     int driveId,
     int candidateId,
     int previousUserId,
     int newUserId,
     bool requireApproval,
     int requestedBy)
    {
        await _candidateRepository.CreateReassignmentAsync(
            driveId,
            candidateId,
            previousUserId,
            newUserId,
            requireApproval,
            requestedBy
        );

        return new Response<RequestReassignmentDto>
        {
            Data = new RequestReassignmentDto
            {
                driveId = driveId,
                candidateId = candidateId,
                previousUserId = previousUserId,
                newUserId = newUserId,
                requireApproval = requireApproval
            }
        };
    }

    #endregion

    #region Private Methods

    private List<CandidateDTO> ConverToDTO(List<Candidate> candidates)
    {
        var candidateDTOs = new List<CandidateDTO>();
        candidates.ForEach(candidate =>
        {
            var candidateDTO = Helper.Map<Candidate, CandidateDTO>(candidate);
            candidateDTO.CandidateExperienceLevel = candidate.ExperienceLevel.ToString();
            candidateDTOs.Add(candidateDTO);
        });
        return candidateDTOs;
    }
    public async Task<List<PanelAssignedCandidateDTO>> GetCandidatesByUserIdAsync(int userId)
    {
        // Business rules can go here if needed
        return await _candidateRepository.GetCandidatesByUserIdAsync(userId);
    }
    #endregion
}