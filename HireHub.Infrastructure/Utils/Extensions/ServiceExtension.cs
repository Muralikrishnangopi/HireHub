using HireHub.Core.Data.Interface;
using HireHub.Core.Utils.Common;
using HireHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HireHub.Infrastructure.Utils.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterInfrastructure(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString(AppSettingKey.DefaultConnection)
           ?? throw new InvalidOperationException(ExceptionMessage.ConnectionStringNotConfigured);
        services.AddDbContext<HireHubDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<ISaveRepository, SaveRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<IDriveRepository, DriveRepository>();
        services.AddScoped<IRoundRepository, RoundRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IAvailbilityRepository, AvailabilityRepository>();

        return services;
    }
}
