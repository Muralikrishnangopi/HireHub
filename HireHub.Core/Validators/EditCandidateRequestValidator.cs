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

        RuleFor(x => x)
            .Custom((req, context) =>
            {
                if (req.ContainsKey(JOPropertyName.Email) && string.IsNullOrWhiteSpace(req[JOPropertyName.Email]!.ToString()))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.EmailShouldNotNull);
                    return;
                }

                if (req.ContainsKey(JOPropertyName.Phone) && string.IsNullOrWhiteSpace(req[JOPropertyName.Phone]!.ToString()))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.PhoneShouldNotNull);
                    return;
                }

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

                if (req.ContainsKey(JOPropertyName.FullName) && string.IsNullOrWhiteSpace(req[JOPropertyName.FullName]!.ToString()))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.NameShouldNotNull);
                    return;
                }

                if (req.ContainsKey(JOPropertyName.ExperienceLevel) &&
                !Options.ExperienceLevels.Contains(req[JOPropertyName.ExperienceLevel]?.ToString()))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidExperienceLevel);
                    return;
                }

                if (req.ContainsKey(JOPropertyName.TechStack) && req[JOPropertyName.TechStack] == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.TechStackShouldNotBeNull);
                    return;
                }
                if (req.ContainsKey(JOPropertyName.TechStack) && req[JOPropertyName.TechStack]!.ToObject<List<string>>() == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.TechStackMustBeList);
                    return;
                }
            });
    }
}
