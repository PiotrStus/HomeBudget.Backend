using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HomeBudget.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Persistance
{
    public static class SqlDatabaseConfiguration
    {
        // rozszerza interfejs IServiceCollection, ktory sluzy do rejestracji w konenterze Dependency Injection
        // jako parametr przyjmuje connection string
        public static IServiceCollection AddSqlDatabase(this IServiceCollection services, string connectionString)
        {
            // akcja ktora zwraca sqlOptions
            // metoda rozszerzajaca UseSqlServer, ktora jako parametr przyjmuje connection string
            Action<IServiceProvider, DbContextOptionsBuilder> sqlOptions = (serviceProvider, options) => options.UseSqlServer(connectionString,
                // dzieki temu jesli wyciagamy ustawienia z kilku tabel to one beda wyciagniete za pomoca jednego query
                // bedzie join zamiast wielu pojedynczych strzalow
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery))
                .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>());

            // AdDdBConxtext, ktora rejestruje w kontenerze pod interfejsem IApplicationDbContext
            // nasza klase MainDbContext, czyli nasza implementacje
            // dzieki czemu wszedzie gdzie bedziemy uzywac IApplicationDbContext, tak naprawde bedziemy
            // pod spodem uzywac klasy MainDbContext, ktora 
            // ma juz dostep do fizycznej bazy danych przez connectionString, ktory przychodzi w opcjach sqlOptions
            services.AddDbContext<IApplicationDbContext, MainDbContext>(sqlOptions);

            return services;
        }
    }
}
