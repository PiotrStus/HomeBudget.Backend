using EFCoreSecondLevelCacheInterceptor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PageMonitor.Application.Interfaces;
using PageMonitor.Infrastructure.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Infrastructure.Auth
{
    public static class AuthConfiguration
    {
        // rozszerza interfejs IServiceCollection, ktory sluzy do rejestracji w konenterze Dependency Injection
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<JwtAuthenticationOptions>(configuration.GetSection("JwtAuthentication"));
            services.AddSingleton<JwtManager>();
            return services;
        }

        // dodajemy metodę do 
        public static IServiceCollection AddPasswordManager(this IServiceCollection services)
        {
            // rejestruje PasswordHasher z tej biblioteki (paczki nuggetowej)
            services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
            // rejestruje implementacje IPasswordManagera w postaci PasswordManagera
            services.AddScoped<IPasswordManager, PasswordManager>();
            return services;
        }
    }
}
