using FluentValidation;
using FluentValidation.Validators;
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
    public class GetPanelAssignedCandidatesRequestValidator : AbstractValidator<GetPanelAssignedCandidatesRequest>
    {
        public GetPanelAssignedCandidatesRequestValidator(
           List<object> warnings,
           RepoService repoService,
           IUserProvider userProvider)
        {
            RuleFor(e => e).Custom((request, context) =>
            {
                // 1️⃣ Logged-in user check
                var currentUserId = userProvider.CurrentUserId;
                var currentUserRole = userProvider.CurrentUserRole;
 
                if (string.IsNullOrEmpty(currentUserId))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.Unauthorized);
                    return;
                }
 
                // 2️⃣ Role must be PANEL
                if (currentUserRole != RoleName.Panel)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.OnlyPanelCanViewAssignedCandidates);
                    return;
                    
                }
 
                // 3️⃣ User must exist
                var userExists = repoService.UserRepository.GetByIdAsync(currentUserId).WaitAsync(CancellationToken.None).Result;
 
 
                if (userExists == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.UserNotFound);
                    return;
                }
 
 
            });
        }
    }
}