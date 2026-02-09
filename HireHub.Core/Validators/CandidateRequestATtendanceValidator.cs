using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;

namespace HireHub.Core.Validators
{
    public class CandidateRequestATtendanceValidator : AbstractValidator<RequestMarkAttendances>
    {
        public CandidateRequestATtendanceValidator(
        List<object> warnings,
        RepoService repoService,
        IUserProvider userProvider)
        {
            RuleFor(x => x.driveId)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidDrive);

            RuleFor(x => x.CandidateId)
                .NotEmpty()
                .WithMessage(ResponseMessage.InvalidCandidate);

            RuleFor(x => x).CustomAsync(async (request, context, ct) =>
            {
                var currentUserId = int.Parse(userProvider.CurrentUserId);

                var driveCandidate =
                    await repoService.CandidateRepository
                        .GetValidDriveCandidateForAttendance(
                            request.driveId,
                            request.CandidateId,
                            currentUserId);

                if (driveCandidate == null)
                {
                    context.AddFailure(
                        PropertyName.Main,
                        ResponseMessage.AttendanceNotAllowed);
                    return;
                }

                if (driveCandidate.Attendance_Status == "Present")
                {
                    context.AddFailure(
                        PropertyName.Main,
                        ResponseMessage.AttendanceAlreadyMarked);
                }
            });
        }
    }
}
