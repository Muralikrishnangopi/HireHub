using HireHub.Core.Utils.Extensions;
using HireHub.Infrastructure.Utils.Extensions;
using Microsoft.OpenApi.Models;

namespace HireHub.Api.Utils.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterSwaggerGen(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Add JWT bearer definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            // Add global security requirement
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection RegisterServices(
        this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddHttpClient();
        services.AddMemoryCache();
        services.AddHttpContextAccessor();

        services.RegisterCore(configuration);

        services.RegisterInfrastructure(configuration);

        return services;
    }
}
