using FluentValidation;
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

                var roleName = req[JOPropertyName.RoleName]?.ToString();
                if (roleName != null && !Options.RoleNames.Contains(roleName))
                {
                    context.AddFailure(PropertyName.Main, ResponseMessage.InvalidRole);
                    return;
                }
            });
    }
}

