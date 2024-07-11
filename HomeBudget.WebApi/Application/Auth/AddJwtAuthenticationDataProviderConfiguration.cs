using EFCoreSecondLevelCacheInterceptor;
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

namespace PageMonitor.WebApi.Application.Auth
{
    public static class AddJwtAuthenticationDataProviderConfiguration
    {
        // rozszerza interfejs IServiceCollection, ktory sluzy do rejestracji w konenterze Dependency Injection
        public static IServiceCollection AddJwtAuthenticationDataProvider(this IServiceCollection services, IConfiguration configuration)
        {
            // zmieniamy sekcje na cookieSettings
            services.Configure<JwtAuthenticationOptions>(configuration.GetSection("CookieSettings"));
            // dodanie implementajci do naszej interfejsu
            // w tym przypadku bedzie to addScope a nie addSingleton            
            // do naszego interfejsu IAuthenticationDataProvider
            // i implementacja bedzie nasza klasa JwtAuthenticationDataProvider
            services.AddScoped<IAuthenticationDataProvider, JwtAuthenticationDataProvider>();
            return services;
        }
    }
}
