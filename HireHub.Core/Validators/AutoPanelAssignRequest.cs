using FluentValidation;
using HireHub.Core.Data.Models;
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
    public class AutoPanelAssignRequest:AbstractValidator<int>
    {
        public AutoPanelAssignRequest(List<object> warnings, RepoService repoService,
        IUserProvider userProvider)
        {
            RuleFor(e => e).NotNull();

            RuleFor(e => e).Custom((driveId, context) =>
            {
                var drivedetail = repoService.DriveRepository.GetByIdAsync(driveId).WaitAsync(CancellationToken.None).Result;
                if(drivedetail!.DriveDate <= DateTime.Today)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.DriveNotYetStart);
                }
                if (drivedetail.Status != DriveStatus.InProposal)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.SomethingHappenInDrive);
                }
            });
        }
    }
}