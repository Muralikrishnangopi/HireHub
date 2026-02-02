using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Validators;

public class EditDriveCandidateRequestValidator : AbstractValidator<JObject>
{
    public EditDriveCandidateRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x[JOPropertyName.DriveId])
            .NotNull().WithMessage(ResponseMessage.DriveIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidDriveId);

        RuleFor(x => x[JOPropertyName.CandidateId])
            .NotNull().WithMessage(ResponseMessage.CandidateIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidCandidateId);

        RuleFor(x => x)
            .Must(x => !x.ContainsKey(JOPropertyName.CreatedDate))
            .WithMessage(ResponseMessage.CreatedDateCannotBeUpdated);

        RuleFor(x => x)
            .Custom((req, context) =>
            {
                int driveId = req[JOPropertyName.DriveId]!.ToObject<int>();
                var drive = repoService.DriveRepository
                    .GetByIdAsync(driveId).WaitAsync(CancellationToken.None).Result;
                if (drive == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNotFound);
                    return;
                }

                var currentUserRole = userProvider.CurrentUserRole;
                var currentUserId = int.Parse(userProvider.CurrentUserId);
                var candidateId = req[JOPropertyName.CandidateId]!.ToObject<int>();
                var isHrInterviewer = repoService.RoundRepository
                    .IsHrInterviewerForCandidateOnDrive(currentUserId, candidateId, driveId);
                if (currentUserRole != RoleName.Admin && currentUserId != drive.CreatedBy && !isHrInterviewer)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.AdminOrDriveOwnerOrHrInterviewerCanEdit);
                    return;
                }

                if (drive.Status == DriveStatus.Completed || drive.Status == DriveStatus.Cancelled)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.ClosedDriveCannotBeEdit);
                    return;
                }

                if (drive.Status == DriveStatus.Halted)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.PausedDriveCannotBeEdit);
                    return;
                }

                var candidate = repoService.CandidateRepository
                    .GetByIdAsync(candidateId).WaitAsync(CancellationToken.None).Result;
                if (candidate == null) 
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.CandidateNotFound);
                    return;
                }

                if (req.ContainsKey(JOPropertyName.CandidateStatus))
                {
                    if (!Options.CandidateStatuses.Contains(req[JOPropertyName.CandidateStatus]!.ToString()))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidCandidateStatus);
                        return;
                    }

                    if (req[JOPropertyName.CandidateStatus]!.ToString() == nameof(CandidateStatus.Pending))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.CandidateStatusCannotChangeToPending);
                        return;
                    }

                    if (drive.Status == DriveStatus.InProposal)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveNeedToStartFirst);
                        return;
                    }
                }
            });
    }
}
