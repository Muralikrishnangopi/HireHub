using FluentValidation;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.UserProgram.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.Validators
{
    public class ReassignPanelAssignRequest:AbstractValidator<ReassignPanel>
    {
        public ReassignPanelAssignRequest(List<object> warnings, RepoService repoService, IUserProvider userProvider)
        {
            RuleFor(x => x.roundId)
                        .GreaterThan(0)
                        .WithMessage("Round Id must be a valid value");

            RuleFor(x => x.oldInterviewId)
            .GreaterThan(0)
            .WithMessage("Old interviewer is required");

            RuleFor(x => x.newInterviewerId)
            .GreaterThan(0)
            .WithMessage("New interviewer is required")
            .NotEqual(x => x.oldInterviewId)
            .WithMessage("New interviewer cannot be same as old interviewer");

           
        }
    }
}