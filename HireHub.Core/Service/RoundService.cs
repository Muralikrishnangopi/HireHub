using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using HireHub.Core.Data.Interface;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using HireHub.Shared.Common.Exceptions;
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
            _logger.LogInformation(LogMessage.StartMethod, nameof(AutoPanelAssign));


            var driveCandidates = await _roundRepository.GetAllDriveCandidates(driveId);
            var interviewerIds = await _roundRepository.GetAllDriveMember(driveId);

            if (!driveCandidates.Any())
                throw new CommonException(ResponseMessage.CandidateNotFound);

            if (!interviewerIds.Any())
                throw new Exception(ResponseMessage.NoPanelMembers);



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
            _logger.LogInformation(LogMessage.EndMethod, nameof(AutoPanelAssign));

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
        public async Task<Response<RoundDTO>> MovetoNextRoundAsync(MovetoNextRoundRequest movetoNextRoundRequest)
        {
            _logger.LogInformation(LogMessage.StartMethod, nameof(MovetoNextRoundAsync));
            var rounds =await  _roundRepository.GetRoundsForDriveCandidate(movetoNextRoundRequest.DriveCandidateId);
            rounds[rounds.Count-1]!.Status = RoundStatus.Completed;
            rounds[rounds.Count-1].Result = RoundResult.Selected;
            var driveId = rounds[0].DriveCandidate!.DriveId;
            var drive = await _driveRepository.GetByIdAsync(driveId);
            var totalRounds = drive!.TechnicalRounds;
            var newRound = new Round() {
                DriveCandidateId = rounds[0].DriveCandidate!.DriveCandidateId,
                InterviewerId = movetoNextRoundRequest.DriveMemberId,
                RoundType = rounds.Count==totalRounds?RoundType.Hr:RoundType.Tech2,
                Status = RoundStatus.Scheduled,
                Result = RoundResult.Pending
            };
            await _roundRepository.AddAsync(newRound,CancellationToken.None);

            _saveRepository.SaveChanges();
            var response = await _roundRepository.GetByIdAsDtoAsync(newRound.RoundId);

            _logger.LogInformation(LogMessage.EndMethod, nameof(MovetoNextRoundAsync));

            return new Response<RoundDTO>{ Data = response };

        }
    }
}