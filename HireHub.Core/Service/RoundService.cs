using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Round = HireHub.Core.Data.Models.Round;

namespace HireHub.Core.Service
{
    public class RoundService
    {
        private readonly ISaveRepository _saveRepository;
        private readonly ILogger<RoundService> _logger;
        private readonly IDriveRepository _driveRepository;
        private readonly IRoundRepository _roundRepository;
        public RoundService(ILogger<RoundService> logger, ISaveRepository saveRepository, IDriveRepository driveRepository, IRoundRepository roundRepository)
        {
            _logger = logger;
            _saveRepository = saveRepository;
            _driveRepository = driveRepository;
            _roundRepository = roundRepository;
        }

        public async Task<List<RoundDTO>> AutoPanelAssign(int driveId)
        {
            var driveCandidates = await _roundRepository.GetAllDriveCandidates(driveId);
            var interviewerIds = await _roundRepository.GetAllDriveMember(driveId);

            if (!driveCandidates.Any())
                throw new Exception("No candidates found");

            if (!interviewerIds.Any())
                throw new Exception("No interviewers found");



            for (int i = 0; i < driveCandidates.Count; i++)
            {
                var round = new Round
                {
                    DriveCandidateId = driveCandidates[i].DriveCandidateId,
                    InterviewerId = interviewerIds[i % interviewerIds.Count].DriveMemberId, // ✅ FIX
                    RoundType = RoundType.Tech1,
                    Status = RoundStatus.Scheduled,
                    Result = RoundResult.Pending
                };
                await _roundRepository.AddAsync(round, CancellationToken.None);
                await _saveRepository.SaveChangesAsync();
            }
            return new();

        }

        public async Task ReassignInterviewer(int roundId, int oldInterviewerId, int newInterviewerId)
        {
            var round = await _roundRepository
                    .GetOldInterviewer(roundId, oldInterviewerId);

            if (round == null)
                throw new Exception("Interviewer not yet assigned");

            await _roundRepository
                .AssignNewInterview(round, newInterviewerId);

            await _saveRepository.SaveChangesAsync();

        }
    }
}