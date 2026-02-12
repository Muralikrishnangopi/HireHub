using FluentValidation;
using HireHub.Core.DTO;
using HireHub.Core.Utils.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HireHub.Core.Validators
{
    public class GetUserRequestValidator:AbstractValidator<DateTime>
    {
        public GetUserRequestValidator(List<object> warnings)
        {
            RuleFor(x=>x).NotNull().WithMessage(ResponseMessage.DriveDateInvalid);
            RuleFor(x => x)
                  .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                  .WithMessage(ResponseMessage.invalidDriveDate);

        }
    }
}
