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
    public class DriveStatusUpdateRequestValidator:AbstractValidator<DriveStatusUpdateRequest>
    {
        public DriveStatusUpdateRequestValidator(List<object> warnings,RepoService repoService,IUserProvider userProvider) 
        {
            RuleFor(x => x.DriveId).NotNull().WithMessage(ResponseMessage.DriveIdRequired);
            RuleFor(x => x.DriveStatus)
                        .NotEmpty()
                        .WithMessage(ResponseMessage.DriveStatusIsRequired)
                        .Must(status =>
                            Enum.TryParse<DriveStatus>(status, true, out _))
                        .WithMessage(ResponseMessage.InvalidDriveStatus);

            RuleFor(x => x).Custom((req, context) =>
            {
                // ✅ Parse safely
                if (!Enum.TryParse<DriveStatus>(req.DriveStatus, true, out var newStatus))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidDriveStatus);
                    return;
                }

                // ✅ Get drive
                var drive =  repoService.DriveRepository
                    .GetByIdAsync(req.DriveId).WaitAsync(CancellationToken.None).Result;

                if (drive is null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNotFound);
                    return;
                }
                if (drive.Status==DriveStatus.InProposal && drive.DriveDate.Date != DateTime.Now.Date)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveCannotStartedBeforeScheduledDate);
                    return;
                }
                if(drive.Status==DriveStatus.Started && drive.DriveDate.Date<= DateTime.Now.Date)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNeedToStartFirst);
                }
                //if (drive.Status == DriveStatus.InProposal)
                //{
                //    context.AddFailure(PropertyName.Main,ResponseMessage.)
                //}
                var currentStatus = drive.Status;

                switch (newStatus)
                {
                    // START
                    case DriveStatus.Started:
                        if (currentStatus != DriveStatus.InProposal)
                        {
                            context.AddFailure(PropertyName.Main, ResponseMessage.DriveStatusCannotBeChangeToStarted);
                        }
                        break;

                    // COMPLETE
                    case DriveStatus.Completed:
                        var allRoundsCompleted = repoService.RoundRepository
                            .AreAllRoundsEvaluatedAsync(req.DriveId).WaitAsync(CancellationToken.None).Result;

                        if (!allRoundsCompleted)
                        {
                            context.AddFailure(PropertyName.Main,ResponseMessage.DriveRoundResultArePending);
                        }
                        break;

                    // CANCEL
                    case DriveStatus.Cancelled:
                        if (currentStatus != DriveStatus.InProposal)
                        {
                            context.AddFailure(PropertyName.Main,
                                ResponseMessage.DriveCannotbeCancelled);
                        }
                        break;
                }
            });
        }
    }
}
