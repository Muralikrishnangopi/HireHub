using FluentValidation;
using HireHub.Core.Data.Models;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;
using Newtonsoft.Json.Linq;

namespace HireHub.Core.Validators;

public class EditUserRequestValidator : AbstractValidator<JObject>
{
    public EditUserRequestValidator(List<object> warnings, RepoService repoService, IUserProvider userProvider)
    {
        RuleFor(x => x[JOPropertyName.UserId])
            .NotNull().WithMessage(ResponseMessage.UserIdRequired)
            .Must(x => int.TryParse(x!.ToString(), out _))
            .WithMessage(ResponseMessage.InvalidUserId);

        RuleFor(x => x)
            .Must(x => !x.ContainsKey(JOPropertyName.PasswordHash))
            .WithMessage(ResponseMessage.PasswordCannotBeUpdated);

        RuleFor(x => x)
            .Must(x => !x.ContainsKey(JOPropertyName.CreatedDate))
            .WithMessage(ResponseMessage.CreatedDateCannotBeUpdated);

        RuleFor(x => x)
            .Must(x => !x.ContainsKey(JOPropertyName.UpdatedDate))
            .WithMessage(ResponseMessage.UpdatedDateCannotBeUpdated);

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
                    repoService.UserRepository
                        .IsUserWithEmailOrPhoneExist(email ?? string.Empty, phone ?? string.Empty)
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

                if (req.ContainsKey(JOPropertyName.IsActive) && req[JOPropertyName.IsActive] == null)
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.IsActiveShouldNotNull);
                    return;
                }

                if (req.ContainsKey(JOPropertyName.RoleName) && !Options.RoleNames.Contains(req[JOPropertyName.RoleName]!.ToString()))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRole);
                    return;
                }
            });
    }
}

