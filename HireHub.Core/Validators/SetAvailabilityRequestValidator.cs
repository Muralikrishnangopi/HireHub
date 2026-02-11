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
    public class SetAvailabilityRequestValidator:AbstractValidator<SetAvailabilityRequest>
    {
        public SetAvailabilityRequestValidator(List<object> warnings)
        {
            RuleFor(x => x.userId)
                        .GreaterThan(0)
                        .WithMessage(ResponseMessage.InvalidUserId);

            RuleFor(x => x.availabilityDates)
                .NotNull()
                .NotEmpty()
                .WithMessage(ResponseMessage.AvailabilityDatesRequired);
            RuleFor(x => x).Custom((req, context) =>
            {
                if (req.availabilityDates != null)
                {
                    req.availabilityDates = req.availabilityDates
                        .Distinct()
                        .ToList();
                }
            });

            RuleForEach(x => x.availabilityDates)
           .Must(date => date.Date >= DateTime.UtcNow.Date.AddDays(7))
           .WithMessage(ResponseMessage.AvailabilityDatesInvalid);
        }
    }
}
