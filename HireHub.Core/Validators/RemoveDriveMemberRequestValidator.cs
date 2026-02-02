using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;

namespace HireHub.Core.Validators;

public class RemoveDriveMemberRequestValidator : AbstractValidator<RemoveDriveMemberRequest>
{
    public RemoveDriveMemberRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x.DriveId)
            .NotNull().WithMessage(ResponseMessage.DriveIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidDriveId);

        RuleFor(x => x.MemberId)
            .NotNull().WithMessage(ResponseMessage.UserIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidUserId);

        RuleFor(x => x)
            .Custom((req, context) =>
            {
                var drive = repoService.DriveRepository
                    .GetDriveWithMembersAsync(req.DriveId).WaitAsync(CancellationToken.None).Result;

                if (drive == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNotFound);
                    return;
                }

                var currentUserId = userProvider.CurrentUserId;
                var currentUserRole = userProvider.CurrentUserRole;
                if (currentUserRole != RoleName.Admin && currentUserId != drive.CreatedBy.ToString())
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.AdminOrDriveOwnerCanRemove);
                    return;
                }

                if (drive.Status == DriveStatus.Completed || drive.Status == DriveStatus.Cancelled)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.ClosedDriveCannotBeEdit);
                    return;
                }

                if (drive.Status != DriveStatus.InProposal)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.CannotRemoveMembersOnStartedDrive);
                    return;
                }

                var driveMember = drive.DriveMembers.FirstOrDefault(e => e.UserId == req.MemberId);
                if (driveMember == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveMemberNotFound);
                    return;
                }
            });
    }
}

