using HireHub.Core.Service;
using HireHub.Core.Utils.Common;
using HireHub.Core.Utils.UserProgram;
using HireHub.Core.Utils.UserProgram.Interface;
using HireHub.Shared.Authentication.Models;
using HireHub.Shared.Extensions;
using HireHub.Shared.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HireHub.Core.Utils.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCore(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        services.AddHireHubShared(
            jwtSettings:
            configuration.GetSection(AppSettingKey.JwtSettings).Get<JwtSettings>() ??
            throw new InvalidOperationException(ExceptionMessage.JwtNotConfigured),
            azureLogicApp:
            configuration.GetSection(AppSettingKey.AzureLogicApp).Get<AzureLogicApp>() ??
            throw new InvalidOperationException(ExceptionMessage.AzureLogicAppNotConfigured)
        );

        services.AddScoped<IUserProvider, UserProvider>();

        services.AddScoped<RepoService>();
        services.AddScoped<TokenService>();
        services.AddScoped<OtpService>();

        services.AddScoped<CommonService>();

        services.AddScoped<UserService>();
        services.AddScoped<AdminService>();
        services.AddScoped<CandidateService>();
        services.AddScoped<DriveService>();

        return services;
    }
}
