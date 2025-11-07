using Microsoft.OpenApi.Models;

namespace TeamFeedbackPro.Controller.Configuration.Extensions
{
    internal static class SwaggerExtensions
    {
        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TeamFeedbackPro API",
                    Version = "v1",
                    Description = "Feedback messeger university project"
                });
                options.CustomSchemaIds(t => t.ToString());
            });

            return services;
        }

        internal static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamFeedbackPro API"); });

            return app;
        }       
    }
}