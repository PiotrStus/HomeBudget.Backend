using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PageMonitor.Application.Interfaces;
using PageMonitor.Application.Services;

namespace PageMonitor.Application
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
    }
}
