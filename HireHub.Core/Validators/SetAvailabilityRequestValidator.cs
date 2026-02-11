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
             .Must(date =>
             {
                 var today = DateTime.UtcNow.Date;

                 // Calculate next Monday
                 int daysUntilNextMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
                 if (daysUntilNextMonday == 0)
                     daysUntilNextMonday = 7;  // if today is Monday, next Monday is 7 days later

                 var nextMonday = today.AddDays(daysUntilNextMonday);

                 // Availability must be >= next Monday
                 return date.Date >= nextMonday;
             })
           .WithMessage(ResponseMessage.AvailabilityDatesInvalid);
        }
    }
}
