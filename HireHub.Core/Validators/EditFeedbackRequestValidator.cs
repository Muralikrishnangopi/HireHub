using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Validators;

public class EditFeedbackRequestValidator : AbstractValidator<JObject>
{
    public EditFeedbackRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
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

                if (req.ContainsKey(JOPropertyName.Recommendation))
                {
                    if (!Options.Recommendations.Contains(req[JOPropertyName.Recommendation]!.ToString()))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidCandidateRecommendation);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.OverallRating))
                {
                    var overallRating = req[JOPropertyName.OverallRating]!.ToObject<int?>();
                    if (overallRating != null && !Options.RatingNumbers.Contains((int)overallRating))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.TechnicalSkill))
                {
                    var technicalSkill = req[JOPropertyName.TechnicalSkill]!.ToObject<int?>();
                    if (technicalSkill != null && !Options.RatingNumbers.Contains((int)technicalSkill))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.Communication))
                {
                    var communication = req[JOPropertyName.Communication]!.ToObject<int?>();
                    if (communication != null && !Options.RatingNumbers.Contains((int)communication))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.ProblemSolving))
                {
                    var problemSolving = req[JOPropertyName.ProblemSolving]!.ToObject<int?>();
                    if (problemSolving != null && !Options.RatingNumbers.Contains((int)problemSolving))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRatingNumber);
                        return;
                    }
                }
            });
    }
}