using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HireHub.Core.Validators;

public class EditDriveRequestValidator : AbstractValidator<JObject>
{
    public EditDriveRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x[JOPropertyName.DriveId])
            .NotNull().WithMessage(ResponseMessage.DriveIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidDriveId);

        RuleFor(x => x)
            .Must(x => !x.ContainsKey(JOPropertyName.CreatedBy))
            .WithMessage(ResponseMessage.CreatedByCannotBeUpdated);

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

                var currentUserId = userProvider.CurrentUserId;
                var currentUserRole = userProvider.CurrentUserRole;
                if (currentUserRole != RoleName.Admin && currentUserId != drive.CreatedBy.ToString())
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.AdminOrDriveOwnerCanEdit);
                    return;
                }

                if (drive.Status == DriveStatus.Completed || drive.Status == DriveStatus.Cancelled)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.ClosedDriveCannotBeEdit);
                    return;
                }

                if (req.ContainsKey(JOPropertyName.DriveName))
                {
                    if (drive.Status != DriveStatus.InProposal)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveNameCannotBeChange);
                        return;
                    }

                    var isDriveNameAlreadyExist = repoService.DriveRepository
                        .IsDriveWithNameExist(req[JOPropertyName.DriveName]!.ToString())
                        .WaitAsync(CancellationToken.None).Result;
                    if (isDriveNameAlreadyExist)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveNameAlreadyExist);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.DriveDate))
                {
                    if (drive.Status != DriveStatus.InProposal)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveDateCannotBeChange);
                        return;
                    }

                    if (req[JOPropertyName.DriveDate]!.ToObject<DateTime>() < DateTime.Today)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.FutureDateOnlyAllowed);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.TechnicalRounds))
                {
                    if (drive.Status != DriveStatus.InProposal)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveTechnicalRoundsCannotBeChange);
                        return;
                    }

                    var techRounds = req[JOPropertyName.TechnicalRounds]!.ToObject<int>();
                    if (techRounds < 0 || techRounds > 2)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.TechRoundsShouldBe);
                        return;
                    }
                }

                if (req.ContainsKey(JOPropertyName.DriveStatus))
                {
                    if (!Options.DriveStatuses.Contains(req[JOPropertyName.DriveStatus]!.ToString()))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.InvalidDriveStatus);
                        return;
                    }

                    if (req[JOPropertyName.DriveStatus]!.ToString() == nameof(DriveStatus.InProposal))
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveStatusCannotChangeToInproposal);
                        return;
                    }

                    if (req[JOPropertyName.DriveStatus]!.ToString() == nameof(DriveStatus.Started) && drive.DriveDate.Date > DateTime.Today)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveCannotStartedBeforeScheduledDate);
                        return;
                    }

                    if (req[JOPropertyName.DriveStatus]!.ToString() == nameof(DriveStatus.Halted) && drive.Status == DriveStatus.InProposal)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.DriveNeedToStartFirst);
                        return;
                    }
                }
            });
    }
}

