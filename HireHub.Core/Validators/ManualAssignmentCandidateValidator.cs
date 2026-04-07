using FluentValidation;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.UserProgram.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.Validators
{
    public class ManualAssignmentCandidateValidator : AbstractValidator<ManualAssignmentCandidate>
    {
        public ManualAssignmentCandidateValidator(List<object> warnings, RepoService repoService,
            IUserProvider userProvider)
        {
            RuleFor(x => x.driveId)
                .GreaterThan(0).WithMessage("DriveId is required");

            RuleFor(x => x.driveCandidateId)
                .GreaterThan(0).WithMessage("DriveCandidateId is required");

            RuleFor(x => x.userId)
                .GreaterThan(0).WithMessage("UserId is required");

            RuleFor(x => x)
                .MustAsync(async (request, cancellation) =>
                {
                    return await repoService.DriveRepository
                        .IsDriveCandidateValidAsync(request.driveId, request.driveCandidateId);
                })
                .WithMessage("DriveCandidate does not belong to the given Drive");

            RuleFor(x => x)
                .MustAsync(async (request, cancellation) =>
                {
                    return await repoService.UserRepository
                        .IsUserPanelForDriveAsync(request.driveId, request.userId);
                })
                .WithMessage("User is not a panel member for this drive");

            RuleFor(x => x)
                .MustAsync(async (request, cancellation) =>
                {
                    var isDuplicate = await repoService.RoundRepository
                        .IsDuplicateAssignmentAsync(request.driveCandidateId, request.userId);

                    return !isDuplicate;
                })
                .WithMessage("This interviewer is already assigned to this candidate");

            RuleFor(x => x)
                .CustomAsync(async (request, context, cancellation) =>
                {
                    var rounds = await repoService.RoundRepository
                        .GetRoundsForDriveCandidate(request.driveCandidateId);

                    if (rounds.Count >= 3)
                    {
                        warnings.Add(new
                        {
                            Message = "Candidate already has multiple rounds assigned"
                        });
                    }
                });
        }
    }
}