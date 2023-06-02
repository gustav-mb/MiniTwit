using Microsoft.OpenApi.Models;

namespace MiniTwit.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "MiniTwit API",
                Version = "v1",
                Description = "A refactor of a Twitter clone handed out in the elective course DevOps on the IT University of Copenhagen.",
                Contact = new OpenApiContact { Name = "Gustav Metnik-Beck", Email = "radiator.k.devops@gmail.com" },
                License = new OpenApiLicense { Name = "MIT Licence", Url = new Uri("https://github.com/gustav-mb/MiniTwit/blob/main/LICENSE") },
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer {token}\"",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                }
            });
        });

        return services;
    }
}