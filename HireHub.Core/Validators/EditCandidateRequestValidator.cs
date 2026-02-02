using FluentValidation;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Validators;

public class EditCandidateRequestValidator:AbstractValidator<JObject>
{
    public EditCandidateRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x[JOPropertyName.CandidateId])
            .NotNull().WithMessage(ResponseMessage.UserIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidCandidateId);

        RuleFor(x => x)
            .Must(x => !x.ContainsKey(JOPropertyName.CreatedDate))
            .WithMessage(ResponseMessage.CreatedDateCannotBeUpdated);

        RuleFor(x => x[JOPropertyName.TechStack])
            .NotNull().When(x => x.ContainsKey(JOPropertyName.TechStack))
                .WithMessage(ResponseMessage.TechStackShouldNotBeNull)
            .Must(x => x![JOPropertyName.TechStack]!.ToObject<List<string>>() != null)
                .When(x => x.ContainsKey(JOPropertyName.TechStack))
                .WithMessage(ResponseMessage.TechStackMustBeList);

        RuleFor(x => x)
            .Custom((req, context) =>
            {
                var email = req[JOPropertyName.Email]?.ToString();
                var phone = req[JOPropertyName.Phone]?.ToString();
                var isAlreadyExist = (email != null || phone != null) ?
                    repoService.CandidateRepository
                        .IsCandidateWithEmailOrPhoneExist(email ?? string.Empty, phone ?? string.Empty)
                        .WaitAsync(CancellationToken.None).Result :
                    false;

                if (isAlreadyExist)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.EmailOrPhoneAlreadyExist);
                    return;
                }

                var experienceLevel = req[JOPropertyName.ExperienceLevel]?.ToString();
                if (experienceLevel != null && !Options.ExperienceLevels.Contains(experienceLevel))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidExperienceLevel);
                    return;
                }
            });
    }
}
