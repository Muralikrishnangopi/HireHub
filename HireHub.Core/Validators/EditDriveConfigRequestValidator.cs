using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Validators;

public class EditDriveConfigRequestValidator : AbstractValidator<JObject>
{
    public EditDriveConfigRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x[JOPropertyName.DriveId])
            .NotNull().WithMessage(ResponseMessage.DriveIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidDriveId);

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
            });
    }
}

