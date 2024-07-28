using EFCoreSecondLevelCacheInterceptor;
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

namespace HomeBudget.WebApi.Application.Auth
{
    public static class AddJwtAuthenticationDataProviderConfiguration
    {
        // rozszerza interfejs IServiceCollection, ktory sluzy do rejestracji w konenterze Dependency Injection
        public static IServiceCollection AddJwtAuthenticationDataProvider(this IServiceCollection services, IConfiguration configuration)
        {
            // zmieniamy sekcje na cookieSettings
            services.Configure<CookieSettings>(configuration.GetSection("CookieSettings"));
            // dodanie implementajci do naszej interfejsu
            // w tym przypadku bedzie to addScope a nie addSingleton            
            // do naszego interfejsu IAuthenticationDataProvider
            // i implementacja bedzie nasza klasa JwtAuthenticationDataProvider
            services.AddScoped<IAuthenticationDataProvider, JwtAuthenticationDataProvider>();
            return services;
        }
    }
}
