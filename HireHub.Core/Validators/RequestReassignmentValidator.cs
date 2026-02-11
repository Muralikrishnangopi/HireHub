using FluentValidation;
using HireHub.Core.Data.Filters;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;

public class RequestReassignmentValidator
    : AbstractValidator<RequestReassignmentDto>
{
    public RequestReassignmentValidator(
        List<object> warnings,
        RepoService repoService,
        IUserProvider userProvider)
    {
        RuleFor(x => x.driveId)
            .GreaterThan(0);

        RuleFor(x => x.candidateId)
            .GreaterThan(0);

        RuleFor(x => x.previousUserId)
            .GreaterThan(0);

        RuleFor(x => x.newUserId)
            .GreaterThan(0);

        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellation) =>
            {
                // 1️⃣ Prevent same user reassignment
                if (request.previousUserId == request.newUserId)
                {
                    context.AddFailure(
                        PropertyName.Main,
                        "Previous and new interviewer cannot be the same");
                    return;
                }

                // 2️⃣ Validate Drive exists
                var drive = await repoService.DriveRepository
                    .GetByIdAsync(request.driveId);

                if (drive == null)
                {
                    context.AddFailure(
                        PropertyName.Main,
                        ResponseMessage.DriveNotFound);
                    return;
                }

                // 3️⃣ Get drive members for this drive
                var members = await repoService.DriveRepository
                    .GetDriveMembersWithDetailsAsync(
                        new DriveMemberFilter
                        {
                            DriveId = request.driveId,
                            IncludePastDrives = true
                        });

                if (!members.Any())
                {
                    context.AddFailure(
                        PropertyName.Main,
                        "No members found for this drive");
                    return;
                }

                // 4️⃣ Check new user is panel (RoleId = 3 in your DB)
                var isPanel = members.Any(m =>
                    m.UserId == request.newUserId &&
                    m.RoleId == 3);

                if (!isPanel)
                {
                    context.AddFailure(
                        PropertyName.Main,
                        "New interviewer is not a panel member");
                }

                // 5️⃣ Optional: Check previous user exists in same drive
                var previousExists = members.Any(m =>
                    m.UserId == request.previousUserId &&
                    m.RoleId == 3);

                if (!previousExists)
                {
                    context.AddFailure(
                        PropertyName.Main,
                        "Previous interviewer is not a panel member of this drive");
                }
            });
    }
}