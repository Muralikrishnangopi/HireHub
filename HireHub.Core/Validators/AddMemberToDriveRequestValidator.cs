using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using HireHub.Shared.Common.Exceptions;

namespace HireHub.Core.Validators;

public class AddMemberToDriveRequestValidator : AbstractValidator<AddMemberToDriveRequest>
{
    public AddMemberToDriveRequestValidator(List<object> warnings, RepoService repoService,
        IUserProvider userProvider)
    {
        RuleFor(e => e.DriveId)
            .NotEmpty();
        RuleFor(e => e.MemberId)
            .NotEmpty();
        RuleFor(e => e.MemberRole)
            .NotEmpty();

        RuleFor(e => e).Custom((request, context) =>
        {
            var drive = repoService.DriveRepository.GetDriveWithMembersAsync(request.DriveId)
                        .WaitAsync(CancellationToken.None).Result;
            if (drive == null)
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.DriveNotFound);
                return;
            }

            var currentUserId = userProvider.CurrentUserId;
            var currentUserRole = userProvider.CurrentUserRole;
            if (currentUserRole != RoleName.Admin && currentUserId != drive.CreatedBy.ToString())
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.AdminOrDriveOwnerCanAdd);
                return;
            }

            if (drive.Status == DriveStatus.Completed || drive.Status == DriveStatus.Cancelled)
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.CannotAddMemberOnClosedDrive);
                return;
            }

            var user = repoService.UserRepository.GetByIdAsync(request.MemberId)
                        .WaitAsync(CancellationToken.None).Result;
            if (user == null)
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.UserNotFound);
                return;
            }
            if (!user.IsActive)
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.InactiveUser);
                return;
            }

            var isAlreadyAdded = drive.DriveMembers.Any(e => e.UserId == request.MemberId);
            if (isAlreadyAdded)
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.AlreadyMemberOfDrive);
                return;
            }

            var alreadyAssignedToSome = repoService.DriveRepository
                .IsUserAssignedInAnyActiveDriveOnDateAsync(user.UserId, drive.DriveDate)
                .WaitAsync(CancellationToken.None).Result;
            if (alreadyAssignedToSome)
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.UsersAssignedToAnotherActiveDriveOnSameDate);
                return;
            }

            var role = repoService.RoleRepository.GetByIdAsync(user.RoleId)
                        .WaitAsync(CancellationToken.None).Result;
            if (role!.RoleName.ToString().ToLower() != request.MemberRole.ToLower())
            {
                context.AddFailure(PropertyName.Main, ResponseMessage.UserNotInSpecifiedRole);
                return;
            }
        });
    }
}