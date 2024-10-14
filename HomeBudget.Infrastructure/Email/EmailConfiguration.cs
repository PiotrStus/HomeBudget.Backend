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

namespace HomeBudget.Infrastructure.Auth
{
    public static class EmailConfiguration
    {
        public static IServiceCollection AddEmailSender(this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, ConsoleEmailSender>();
            return services;
        }
    }
}