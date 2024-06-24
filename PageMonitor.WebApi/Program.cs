using PageMonitor.Application.Logic.Abstractions;
using PageMonitor.Infrastructure.Persistance;
using PageMonitor.WebApi.Middlewares;
using Serilog;
//trzeba bylo dodac using recznie
using PageMonitor.Application;
using System;
using PageMonitor.Infrastructure.Auth;
using PageMonitor.WebApi.Application.Auth;

namespace PageMonitor.WebApi
{
    public class Program
    {
        public static string APP_NAME = "PageMonitor.WebApi";
        public static void Main(string[] args)
        {

            //logi problemow ktore sie pojawia zanim w ogole aplikacja sie odpali
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Application", APP_NAME)
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.FromLogContext()
                // na sztywno loguje do konsoli
                .WriteTo.Console()
                // konfiguruje logger w taki sposob, aby moc zalogowac bledy zanim app na dobre wystartuje
                .CreateBootstrapLogger();

            var builder = WebApplication.CreateBuilder(args);

            // nadpisuje wszystkie ustawienia 
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddJsonFile("appsettings.Development.local.json");
            }


            // context - 
            // services - konener z dependency injection
            // configuration - lokalna konfiguracja
            builder.Host.UseSerilog((context, services, configuration) => configuration
                // do kazdego logu dopisujemy property o nazwie Application z nazwa aplikacji
                .Enrich.WithProperty("Application", APP_NAME)
                // dodanie do kazdego logu z informacja jaka jest nazwa komputera, ktora wystawia te logi
                // przydatne gdy wiele instancji aplikacji, albo wiele aplikacji, ktore loguja do tego samego miejsca
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                // dodatkowe ustawienia bedziemy mogli odczytac z pliku appsettings.Development.json
                .ReadFrom.Configuration(context.Configuration)
                // pozwala odczytac ustawienia z konetnera dependency injection, bo pewne ustawienia moga byc wstrzykniete
                .ReadFrom.Services(services)
                // dzieki temu mozna fragment kodu opakowac w using i tam np. dodac dodatkowe property
                .Enrich.FromLogContext());

            // Add services to the container.


            // standardowa metoda rozszerzajaca z frameworka
            // dzieki temu moze IHttpContextAccessor jest zarejestrownay
            // w kontenrze Dependency Injection i mozna go wstrzykiwac jak
            // private readonly IHttpContextAccessor _httpContextAccessor
            // w JwtAuthenticationDataProvider(JwtManager jwtManager, IHttpContextAccessor httpContextAccessor)
            builder.Services.AddHttpContextAccessor(); 


            builder.Services.AddDatabaseCache();
            // wywolanie metody rozszerzajacej, ktora zarejestruje EF i wszystkie ustawienia w kontenerze Dependency Injection
            builder.Services.AddSqlDatabase(builder.Configuration.GetConnectionString("MainDbSql")!);

            builder.Services.AddControllers();
            builder.Services.AddJwtAuth(builder.Configuration);

            // wywyolujemy metode rozszerzajaca 
            builder.Services.AddJwtAuthenticationDataProvider(builder.Configuration);


            builder.Services.AddPasswordManager();

            builder.Services.AddMediatR(c =>
            {
                c.RegisterServicesFromAssemblyContaining(typeof(BaseCommandHandler));
            });

            // wywolanie metody
            builder.Services.AddApplicationServices();

            // wywolanie walidatorow
            builder.Services.AddValidators();

            // swagger
            builder.Services.AddSwaggerGen(o =>
            {
                o.CustomSchemaIds(x =>
                {
                    var name = x.FullName;
                    if (name != null)
                    {
                        // fix na bugga, ktory jest w swaggerze
                        // my uzywamy klas statycznych do commands
                        // i klas request results, ktore sa zagniezdzone w klasie staycznej
                        // i one wtedy w nazwie typu, ktora .NET generuje jak serializuje
                        // nazwe do stringa zawieraja + natomist swagger nie umie takiej
                        // nazwy z + obsluzyc, dlatego musimy tutaj taka castomowa konfiguracje
                        // dodac, ktora zamienia + n a _
                        // ten blad wystepuje od dawna w swaggerze i nikt go nie naprawia
                        // i trzeba po prostu dodac takiego fixa
                        name = name.Replace("+", "_"); // swagger bug fix
                    }

                    return name;
                });
            });


            var app = builder.Build();

            // juz po buildzie dodajemy samo u¿ycie tego swaggera
            // i swaggera ui, dzieki czemu bedziemy mogli interefjs webowy sobie
            // potestowac nasze akcje
            // robimy to tylko na srodowisku developrskim poniewaz
            // nie chcemy tego interfejsu na produkcji gdzies tam ludziom pokazywac


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseExceptionResultMiddleware();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
