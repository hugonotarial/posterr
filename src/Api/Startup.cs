using Api.Extensions;
using Api.Middlewares;
using Infra.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var settings = services.AddSettings(Configuration);

            services
                .AddData()
                .AddValidator()
                .AddCustomHealthCheck(settings)
                .AddSwagger()
                .AddRepository()
                .AddApplicationService()
                .AddAutoMapperService()
                .AddControllers(options => options.Filters.Add(new ExceptionFilter()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwaggerServices();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.UseCustomMapHealthChecksEndpoint();
            });
        }
    }
}
