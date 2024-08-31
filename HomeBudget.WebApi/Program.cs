using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Infrastructure.Persistance;
using HomeBudget.WebApi.Middlewares;
using Serilog;
//trzeba bylo dodac using recznie
using HomeBudget.Application;
using System;
using HomeBudget.Infrastructure.Auth;
using HomeBudget.WebApi.Application.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Transactions;

namespace HomeBudget.WebApi
{
    public class Program
    {
        public static string APP_NAME = "HomeBudget.WebApi";
        public static void Main(string[] args)
        {
            TransactionManager.ImplicitDistributedTransactions = true;

            //logi problemow ktore sie pojawia zanim w ogole aplikacja sie odpali
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Application", APP_NAME)
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.FromLogContext()
                // na sztywno loguje do konsoli
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341")
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


            // to jest aby antyforgery token dzialal w ASP .NET Corze
            builder.Services.AddControllersWithViews(options =>
                {
                    // nie dodajemy na developerskim, bo jak sie doda
                    // na developerskim to przestana dzialac akcje ze swaggera
                    // bo wtedy w swagerze tokena nie bedzie
                    // trzeba bedzie w swaggerze strzelac po token
                    // potem ten token w naglowku przkazywac i to komplikuje testy
                    if (!builder.Environment.IsDevelopment())
                    {
                        // mozemy dodac filter do kazdej akcji
                        // to jest filtr z frameworka
                        // bedzie sie automatycznie odpalal dla wszystkich akcji POST, PUT, DELETE
                        // nie bedzie sie odpalal dla GET i bodajze OPTIONS
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }
                // pozwoli to nam skonfigurowac ustawienia serializacji/deserializacji jsona
                // uzywanego przy odpowiedziach i przy parsowaniu requestow do akcji controllera
                // JsonStringEnumConverter -> wbudowana klasa, dzieki czemu bedziemy mogli
                // w przypadku, gdy w naszych requestach bedziemy wymagac podania enuma
                // bedziemy mogli podac tez wartosci tekstowe zamiast liczbowych
                // dzieki temu duzo latwiej sie czyta takie requesty i duzo czytelniejszy 
                // kod z aplikacji frontendowej jest napisany, poniewaz zamiast liczb mozemy uzyc
                // wartosci tekstowych
                // koniec
                }).AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

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


            // w ustawieniach podajemy nazwê nag³owka, w ktorym bedzie przychodzil nasz token 
            // ktory framework bedzie sprawdzal
            // czy zgadza sie z tokenem wygenerowanym wczesniej u nas w aplikacji
            builder.Services.AddAntiforgery(o =>
            {
                o.HeaderName = "X-XSRF-TOKEN";
            });

            // dodajemy do serwisow obsluge CORS ona jest wbudowana w ASP .NET Core
            builder.Services.AddCors();



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


            app.UseCors(builder => builder
                // pozwalamy na komunikacje z naszym api tylko z domen, które
                // bêd¹ w naszej aplikacji dodane pod kluczem WebAppBaseUrl
                // pozniej tego klucza bedziemy uzywac np. do generowania linkow
                // do naszej aplikacji frontendowej
                // wiec WebAppBaseUrl to jest po prostu adres aplikacji frontendowej
                // ktora bedziemy tworzyc pozniej
                .WithOrigins(app.Configuration.GetValue<string>("WebAppBaseUrl") ?? "")
                // dodajemy tez domeny, ktore sa w AdditionalCorsOrigins
                // to sie czasem przydaje, kiedy chcemy sie dostawac do naszego api
                // z kilku domen, np. na jakis testach albo po prostu chcemy gdzies sie
                // na chwile wpiac do naszego api
                // na produkcji to ustawienie powinno zostac puste
                .WithOrigins(app.Configuration.GetSection("AdditionalCorsOrigins").Get<string[]>() ?? new string[0])
                // to jest to samo, ale odczytujemy jeszcze domeny ze zmiennej srodowiskowej
                // ktore sa oddzielone przecinkiem, to ulatwia konfiguracje
                // np. w Azurze, latwiej podac liste takich domen
                .WithOrigins((Environment.GetEnvironmentVariable("AdditionalCorsOrigins") ?? "").Split(',').Where(h => !string.IsNullOrEmpty(h)).Select(h => h.Trim()).ToArray())
                // pozwalamy na komunikacji z dowolnym naglowkiem
                .AllowAnyHeader()
                // opcja niezbedna po to zeby cookies sie wysylaly
                // i ustawialy z naszego api
                .AllowCredentials()
                // zezwalamy na kazda metode: get, post itd.
                .AllowAnyMethod());


            app.UseExceptionResultMiddleware();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
