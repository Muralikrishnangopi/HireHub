using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
namespace HireHub.Core.Validators;

public class AddFeedbackRequestValidator : AbstractValidator<AddFeedbackRequest>
{
    public AddFeedbackRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x.RoundId)
            .NotNull().WithMessage(ResponseMessage.RoundIdRequired);

        RuleFor(x => x)
            .Custom((req, context) =>
            {
                var round = repoService.RoundRepository
                    .GetRoundByIdWithDetails(req.RoundId).WaitAsync(CancellationToken.None).Result;
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
                    .IsInterviewerForRound(currentUserId, req.RoundId);
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

                if (!Options.Recommendations.Contains(req.CandidateRecommendation))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidCandidateRecommendation);
                    return;
                }

                if (req.OverallRating != null && !Options.RatingNumbers.Contains((int)req.OverallRating))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                    return;
                }

                if (req.TechnicalSkill != null && !Options.RatingNumbers.Contains((int)req.TechnicalSkill))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                    return;
                }

                if (req.Communication != null && !Options.RatingNumbers.Contains((int)req.Communication))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                    return;
                }

                if (req.ProblemSolving != null && !Options.RatingNumbers.Contains((int)req.ProblemSolving))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                    return;
                }
            });
    }
}