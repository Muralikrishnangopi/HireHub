using FluentValidation;
using HireHub.Core.DTO;
using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram.Interface;

namespace HireHub.Core.Validators;

public class AddUserRequestValidator : AbstractValidator<AddUserRequest>
{
    public AddUserRequestValidator(List<object> warnings, RepoService repoService,
        IUserProvider userProvider)
    {
        RuleFor(e => e.FullName).NotEmpty();
        RuleFor(e => e.Email).NotEmpty();
        RuleFor(e => e.Phone).NotEmpty();
        RuleFor(e => e.RoleName)
            .NotEmpty()
            .Must(e => Options.RoleNames.Contains(e)).WithMessage(ResponseMessage.InvalidRole);
        RuleFor(e => e.Password)
            .NotEmpty()
            .MinimumLength(8);

        RuleFor(e => e).Custom((request, context) =>
        {
            var isEmailOrPhoneExist = repoService.UserRepository
            .IsUserWithEmailOrPhoneExist(request.Email, request.Phone)
            .WaitAsync(CancellationToken.None).Result;

            if (isEmailOrPhoneExist)
                context.AddFailure(PropertyName.Main, ResponseMessage.EmailOrPhoneAlreadyExist);
        });
    }
}
