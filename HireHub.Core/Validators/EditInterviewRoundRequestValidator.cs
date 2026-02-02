using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Validators;

public class EditInterviewRoundRequestValidator : AbstractValidator<JObject>
{
    public EditInterviewRoundRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x[JOPropertyName.RoundId])
            .NotNull().WithMessage(ResponseMessage.RoundIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidRoundId);

        RuleFor(x => x)
            .Custom((req, context) =>
            {
                int roundId = req[JOPropertyName.RoundId]!.ToObject<int>();
                var round = repoService.RoundRepository
                    .GetRoundByIdWithDetails(roundId).WaitAsync(CancellationToken.None).Result;
                if (round == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InterviewRoundNotFound);
                    return;
                }

                var drive = repoService.DriveRepository.GetByIdAsync(round.Interviewer!.DriveId)
                            .WaitAsync(CancellationToken.None).Result;

                if (drive == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNotFound);
                    return;
                }

                var currentUserRole = userProvider.CurrentUserRole;
                var currentUserId = int.Parse(userProvider.CurrentUserId);
                var isInterviewerForRound = repoService.RoundRepository
                    .IsInterviewerForRound(currentUserId, roundId);
                if (currentUserRole != RoleName.Admin && currentUserId != drive.CreatedBy && !isInterviewerForRound)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.AdminOrDriveOwnerOrInterviewerCanEdit);
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

                if (drive.Status == DriveStatus.InProposal)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNeedToStartFirst);
                    return;
                }

                if (round.Status == RoundStatus.Completed || round.Status == RoundStatus.Skipped)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InterviewRoundClosed);
                    return;
                }

                if (req.ContainsKey(JOPropertyName.RoundStatus))
                {
                    if (!Options.RoundStatuses.Contains(req[JOPropertyName.RoundStatus]!.ToString()))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRoundStatus);
                        return;
                    }

                    if (req[JOPropertyName.RoundStatus]!.ToString() == nameof(RoundStatus.Completed) && round.Result == RoundResult.Pending)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.NeedToSetRoundResultBeforeCloseRound);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.RoundResult))
                {
                    if (!Options.RoundResults.Contains(req[JOPropertyName.RoundResult]!.ToString()))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRoundResult);
                        return;
                    }

                    if (round.Status == RoundStatus.Scheduled)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.NeedToStartRoundBeforeSetRoundResult);
                        return;
                    }
                }
            });
    }
}
