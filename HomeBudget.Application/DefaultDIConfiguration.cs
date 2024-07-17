using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Application.Logic.Validators;
using HomeBudget.Application.Services;

namespace HomeBudget.Application
{
    public static class DefaultDIConfiguration
    {
        // AddApplicationServices -> metoda rozszerzenia dla IServiceCollection
        // IServiceCollection jest interfejsem używanym do rejestracji usług w kontenerze DI.
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // w tej metodzie dodajemy implementacje do interfejsu za pomocą klas w folderze Services
            // AddScoped<ICurrentAccountProvider, CurrentAccountProvider>() rejestruje implementację
            // CurrentAccountProvider jako usługę typu ICurrentAccountProvider w zakresie "scoped"
            services.AddScoped<ICurrentAccountProvider, CurrentAccountProvider>();
            // metoda AddApplicationServices zwraca zaktualizowaną kolekcję usług IServiceCollection po dodaniu nowych usług.
            
            // kolejne serwisy dodawac bedziemy w kolejnych linijkach
            
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            // dodaje walidatory z assembly, ktore zawiera klase BaseQueryHandler
            // czyli to bedzie Assembly Application
            // jest to metoda rozszerzajaca z fluentValidation
            // ona autoamtycznie rejestruje wszystkie walidatory, ktore
            // dziedzicza po klasie abstract Validator
            services.AddValidatorsFromAssemblyContaining(typeof(BaseQueryHandler));
            // tutaj dodajemy IPipelineBehavior do mediatora
            // czyli wpinamy sie w proces przetwarzania command i query tym naszym
            // typeof(ValidationBehavior, ktore moze odpalac jakis kod przed albo po
            // command i query no i wlasnie my to robimy przed
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            // to nam zalatwia walidacje dla kazdego commanda i query osobno
            // jedyne co to musimy dodac klase walidatora a reszta zadziala magicznie sama
            return services;
        }
    }
}
