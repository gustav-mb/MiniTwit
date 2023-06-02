using Microsoft.OpenApi.Models;

namespace MiniTwit.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
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
                License = new OpenApiLicense { Name = "MIT Licence", Url = new Uri("https://github.com/gustav-mb/MiniTwit/blob/main/LICENSE")},
            });
        });

        return services;
    }
}