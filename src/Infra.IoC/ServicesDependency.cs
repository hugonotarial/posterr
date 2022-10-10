using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Domain.Setting.Interface;
using Domain.Setting;
using Infra.Data.Interface;
using Infra.Data;
using Infra.Data.Repository;
using Microsoft.OpenApi.Models;
using ApplicationService;
using ApplicationService.Interface;
using Infra.Data.Interface.Repository;
using ApplicationService.Mapping;
using FluentValidation.AspNetCore;
using ApplicationService.Validation;
using ApplicationService.Interface.Validation;

namespace Infra.IoC
{
    public static class ServicesDependency
    {
        public static IProjectSettings AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ProjectSettings
            {
                DbConnectionString = configuration.GetConnectionString("PosterrDatabase")
            };
            services.AddSingleton<IProjectSettings>(settings);
            return settings;
        }

        public static IServiceCollection AddData(this IServiceCollection services)
        {
            services.AddScoped<IDbContext, DbContext>();
            return services;
        }

        public static IServiceCollection AddValidator(this IServiceCollection services)
        {
            services.AddFluentValidation();
            services.AddScoped<IUserValidator, UserValidator>();
            services.AddScoped<IPostValidator, PostValidator>();
            return services;
        }

        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IProjectSettings settings)
        {
            services.AddHealthChecks()
               .AddSqlServer(settings.DbConnectionString,
                   name: "ApplicationDatabase", tags: new string[] { "ready" });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Posterr Service Api", Version = "v1" });
                var filePath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
                options.IncludeXmlComments(filePath);
            });

            return services;
        }

        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IFollowerAppService, FollowerAppService>();
            services.AddScoped<IPostAppService, PostAppService>();
            services.AddScoped<IUserAppService, UserAppService>();
            return services;
        }

        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IFollowerRepository, FollowerRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddAutoMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfile));
            return services;
        }
    }
}
