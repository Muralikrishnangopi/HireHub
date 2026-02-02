using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using Microsoft.Extensions.Logging;

namespace HireHub.Core.Service;

public class CommonService
{
    private readonly ICandidateRepository _candidateRepository;
    private readonly ISaveRepository _saveRepository;
    private readonly ILogger<CommonService> _logger;

    public CommonService(ICandidateRepository candidateRepository,
        ISaveRepository saveRepository, ILogger<CommonService> logger)
    {
        _candidateRepository = candidateRepository;
        _saveRepository = saveRepository;
        _logger = logger;
    }


    #region Query Services

    

    #endregion

    #region Command Services

    public async Task<Response<List<int>>> InsertCandidatesBulk(List<AddCandidateRequest> request)
    {
        _logger.LogInformation(LogMessage.StartMethod, nameof(InsertCandidatesBulk));

        var candidates = new List<Candidate>();
        request.ForEach(req =>
        {
            var candidate = Helper.Map<AddCandidateRequest, Candidate>(req);
            candidate.ExperienceLevel = (CandidateExperienceLevel)Enum
                .Parse(typeof(CandidateExperienceLevel), req.ExperienceLevelName, true);
            candidates.Add(candidate);
        });

        await _candidateRepository.BulkInsertAsync(candidates, CancellationToken.None);
        _saveRepository.SaveChanges();

        var ids = new List<int>();
        candidates.ForEach(c => ids.Add(c.CandidateId));

        _logger.LogInformation(LogMessage.EndMethod, nameof(InsertCandidatesBulk));

        return new() { Data = ids };
    }

    #endregion

    #region Private Methods

    

    #endregion
}