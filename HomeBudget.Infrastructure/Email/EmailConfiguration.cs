using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using HomeBudget.Application.Interfaces;
using HomeBudget.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Infrastructure.Email;
using HomeBudget.Infrastructure.Email.Template;
using HomeBudget.Infrastructure.Email.Templates;

namespace HomeBudget.Infrastructure.Auth
{
    public static class EmailConfiguration
    {
        public static IServiceCollection AddEmailServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, ConsoleEmailSender>();
            services.AddScoped<ITemplateProvider, TemplateProvider>();
            services.AddScoped<ITemplateRenderer, TemplateRenderer>();

            return services;
        }
    }
}