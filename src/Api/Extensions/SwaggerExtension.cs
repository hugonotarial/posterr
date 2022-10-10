using Microsoft.AspNetCore.Builder;

namespace Api.Extensions
{
    public static class SwaggerExtension
    {
        public static void UseSwaggerServices(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
        }
    }
}
