using DocumentFormat.OpenXml.Drawing;
using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.Validators
{
    public class MovetoNextRoundValidator:AbstractValidator<MovetoNextRoundRequest>
    {
        public MovetoNextRoundValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
        {
            RuleFor(x => x.UserId).NotNull().WithMessage(ResponseMessage.UserIdRequired);
            RuleFor(x=>x.DriveCandidateId).NotNull().WithMessage(ResponseMessage.CandidateIdRequired);
            RuleFor(x => x.DriveMemberId).NotNull().WithMessage(ResponseMessage.UserIdRequired);
            RuleFor(x=>x.RoundId).NotNull().WithMessage(ResponseMessage.RoundIdRequired);
            RuleFor(x => x).Custom((req, context) =>
            {
                var rounds=repoService.RoundRepository.GetRoundsForDriveCandidate(req.DriveCandidateId).WaitAsync(CancellationToken.None).Result;
                if(rounds is null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InterviewRoundNotFound);
                    return;
                }
                var driveId = rounds[0].DriveCandidate!.DriveId;
                var drive=repoService.DriveRepository.GetByIdAsync(driveId).WaitAsync(CancellationToken.None).Result;
                if(drive is null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNotFound);
                }
                if (drive!.Status == DriveStatus.Completed || drive.Status == DriveStatus.Cancelled)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.ClosedDriveCannotBeEdit);
                    return;
                }
                var result=repoService.DriveRepository.IsUserAssignedWithDriveId(req.UserId,driveId).WaitAsync(CancellationToken.None).Result;
                if(!result)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.UserMustbeInSameDrive);
                    return;
                }
                for(int i = 0; i < rounds.Count; i++) 
                {
                    if (req.DriveMemberId == rounds[i].InterviewerId)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.SameInterviewerId);
                        return;
                    }
                }

            });

        }
    }
}
