using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Infrastructure.Persistance
{
    public static class CacheConfiguration
    {
        public static IServiceCollection AddDatabaseCache(this IServiceCollection services)
        {
            services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(5))
                    .ConfigureLogging(false)
                    .UseCacheKeyPrefix("EF_")
            );

            return services;
        }
    }
}
