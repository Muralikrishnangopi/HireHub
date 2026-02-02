using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;

namespace HireHub.Core.Validators;

public class CreateDriveRequestValidator : AbstractValidator<CreateDriveRequest>
{
    public CreateDriveRequestValidator(List<object> warnings, RepoService repoService,
        IUserProvider userProvider)
    {
        RuleFor(x => x.DriveName).NotEmpty().MaximumLength(150);

        RuleFor(x => x.DriveDate).GreaterThanOrEqualTo(DateTime.Today);

        RuleFor(x => x.TechnicalRounds).GreaterThan(0).LessThanOrEqualTo(2);

        RuleFor(x => x.CoordinationTeam).NotNull();
        RuleFor(x => x.CoordinationTeam.Hrs).NotEmpty().WithMessage(ResponseMessage.NoHrs);
        RuleFor(x => x.CoordinationTeam.PanelMembers).NotEmpty().WithMessage(ResponseMessage.NoPanelMembers);
        RuleFor(x => x.CoordinationTeam.Mentors).NotEmpty().WithMessage(ResponseMessage.NoMentors);

        RuleFor(x => x.HrConfiguration).NotNull();
        RuleFor(x => x.MentorConfiguration).NotNull();
        RuleFor(x => x.PanelConfiguration).NotNull();
        RuleFor(x => x.PanelVisibilityConfiguration).NotNull();
        RuleFor(x => x.NotificationConfiguration).NotNull();
        RuleFor(x => x.FeedbackSettings).NotNull();

        RuleFor(x => x).Custom((request, context) =>
        {
            bool CheckUserAndRole(List<int> userIds, int roleId)
            {
                foreach (int userId in userIds)
                {
                    var user = repoService.UserRepository.GetByIdAsync(userId)
                        .WaitAsync(CancellationToken.None).Result;
                    if (user == null)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.SomeUserNotFound);
                        return false;
                    }
                    if (!user.IsActive)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.SomeInactiveUsersFound);
                        return false;
                    }
                    if (user.RoleId != roleId)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.SomeUserNotInSpecifiedRole);
                        return false;
                    }

                    var alreadyAssigned = repoService.DriveRepository
                    .IsUserAssignedInAnyActiveDriveOnDateAsync(userId, request.DriveDate)
                    .WaitAsync(CancellationToken.None).Result;
                    if (alreadyAssigned)
                    {
                        context.AddFailure(PropertyName.Main, ResponseMessage.SomeUsersAssignedToAnotherActiveDriveOnSameDate);
                        return false;
                    }
                }
                return true;
            }

            var hrRole = repoService.RoleRepository.GetByName(UserRole.HR)
                .WaitAsync(CancellationToken.None).Result;
            if (!CheckUserAndRole(request.CoordinationTeam.Hrs, hrRole.RoleId))
                return;
            var panelRole = repoService.RoleRepository.GetByName(UserRole.Panel)
                .WaitAsync(CancellationToken.None).Result;
            if (!CheckUserAndRole(request.CoordinationTeam.PanelMembers, panelRole.RoleId))
                return;
            var mentorRole = repoService.RoleRepository.GetByName(UserRole.Mentor)
                .WaitAsync(CancellationToken.None).Result;
            if (!CheckUserAndRole(request.CoordinationTeam.Mentors, mentorRole.RoleId))
                return;
        });

        RuleFor(x => x).Custom((request, context) =>
        {
            void CheckDuplicates(List<int> userIds, string roleName)
            {
                var duplicates = userIds
                    .GroupBy(x => x)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key);

                if (duplicates.Any())
                    context.AddFailure(PropertyName.Main, 
                        string.Format(ResponseMessage.SomeDuplicateUsersFoundIn, roleName)
                    );
            }

            CheckDuplicates(request.CoordinationTeam.Hrs, RoleName.Hr);
            CheckDuplicates(request.CoordinationTeam.Mentors, RoleName.Mentor);
            CheckDuplicates(request.CoordinationTeam.PanelMembers, RoleName.Panel);
        });
    }
}
