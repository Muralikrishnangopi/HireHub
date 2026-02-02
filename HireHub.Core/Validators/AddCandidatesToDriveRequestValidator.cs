using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;

namespace HireHub.Core.Validators;

public class AddCandidatesToDriveRequestValidator : AbstractValidator<AddCandidatesToDriveRequest>
{
    public AddCandidatesToDriveRequestValidator(List<object> warnings, RepoService repoService,
        IUserProvider userProvider)
    {
        RuleFor(e => e.DriveId)
            .NotEmpty();
        RuleFor(e => e.CandidateIds)
            .NotNull();

        RuleFor(e => e).Custom((request, context) =>
        {
            var drive = repoService.DriveRepository.GetByIdAsync(request.DriveId)
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
                context.AddFailure(PropertyName.Main, ResponseMessage.CannotAddCandidatesOnClosedDrive);
                return;
            }
        });
    }
}