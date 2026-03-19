using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.Validators
{
    public class CandidateStatusUpdateRequestValidator:AbstractValidator<CandidateStatusUpdateRequest>
    {
        public CandidateStatusUpdateRequestValidator(List<object> warnings,RepoService repoService, IUserProvider userProvider)
        {
            RuleFor(x => x.RoundId).NotNull().WithMessage(ResponseMessage.RoundIdRequired);
            RuleFor(x => x.CandidateStatus).NotNull().WithMessage(ResponseMessage.CandidateStatusIsRequired);
            RuleFor(e => e.CandidateStatus)
           .NotEmpty()
           .Must(e => Options.CandidateStatuses.Contains(e)).WithMessage(ResponseMessage.InvalidCandidateStatus);
            RuleFor(x => x).Custom((req, context) =>
            {
                var round = repoService.RoundRepository.GetByIdAsync(req.RoundId).WaitAsync(CancellationToken.None).Result;
                if (round is null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InterviewRoundNotFound);
                    return;
                }
                if (round.Status != RoundStatus.OnProcess)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRoundStatus);
                    return;
                }
            });
        }
    }
}
