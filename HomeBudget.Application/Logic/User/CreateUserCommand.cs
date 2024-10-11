using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HomeBudget.Application.Logic.User.CreateUserCommand.Request;

namespace HomeBudget.Application.Logic.User
{
    public static class CreateUserCommand
    {
        // tworzymy w środku klasy, które zawsze mają takie same nazwy
        // niezależnie jak nazywa się ta klasa statyczna u góry
        // czyli niezaleznie jaki to jest command

        // Request jest na gorze, bo tak latwiej sie czyta kod
        // ona implementuje interfejs mediatora
        // bedziemy tutaj do wywolywania tych command i query
        // uzywac mediatora
        // ona implementuje request IRequest typu Result
        public class Request : IRequest<Result>
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }


        // Result zwraca dane, który zawiera dany command
        // w tym przypadku jest to Id usera
        public class Result
        {
            public required int UserId { get; set; }
        }


        // dodajemy handler do oblusgi z mediatora
        // bazuje na klasie BaseCommandHandler (posiada rozne wymagane obiekty, potrzebne w kazdym commandzie)
        // oraz impelemntuje interfejs tez z mediatora IRequestHandler od klas
        // Request i Result
        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IPasswordManager _passwordManager;

            // te wszystie obiekty: currentAccountProvider, applicationDbContext 
            // beda wstrzykniete przez DI, wiec nie musimy tutaj nic z nimi robic
            // natomiast do dzialania klasy potrzebujemy jeszcze PasswordManagera wiec go dokladamy do paramaetrow
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, IPasswordManager passwordManager) : base(currentAccountProvider, applicationDbContext)
            {
                _passwordManager = passwordManager;
            }


            // tutaj obslugujemy logike tworzenia takiego konta
            // czyli jesli przyjdzie command z uzytkownikiem to tutaj musimy zawrzec 
            // cala logike tworzenia takiego konta
            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                // pierwsze co robimy to sprawdzamy w bazie czy istnieje juz taki uzytkownik
                var userExists = await _applicationDbContext.Users.AnyAsync(u => u.Email == request.Email);
                if (userExists)
                {
                    // rzucamy wyjatek, ze konto o danym adresie email juz istnieje
                    throw new ErrorException("AccountWithThisEmailAlreadyExists");
                }

                // kolejny krok to jest utworzenie nowej encji uzytkownika i dodanie jej do
                // kontekstu Entity Frameworka
                // czyli jesli uzytkownik nie istnieje to robimy cos takiego

                // bierzemy date aktualna
                var utcNow = DateTime.UtcNow;

                // tworzymy nowego uzytkownika
                var user = new Domain.Entities.User()
                {
                    RegisterDate = utcNow,
                    // email to to co podal uzytkownik
                    Email = request.Email,
                    // jako haslo podajemy ciag pusty,
                    // w pliku DomainEntity to pole HashedPassword jest typu required
                    // wiec i tak trzeba je podac, zeby utworzyc w ogole obiekt
                    // wiec narazie podajemy pusty
                    HashedPassword = "",
                };

                // natomiast za chwile przypisujemy tutaj, z tego ktore podal uzytkownik
                // wykorzystujac password managera
                user.HashedPassword = _passwordManager.HashPassword(request.Password);

                // nastepnie dodajemy do contextu EF do kolekcji users danego uzytkownika
                // on tutaj sie jeszcze nie zapisal do bazy danych
                // jest narazie dodany tutaj do pamieci 
                // natomiast zapis nastapi pozniej 
                _applicationDbContext.Users.Add(user);



                await _applicationDbContext.SaveChangesAsync(cancellationToken);


                // po tym wszystkich potrzebujemy jeszcze zwrocic rezultat
                // ktory wymaga id usera
                
                return new Result()
                { 
                    // jest to Id usera, ktore jest nadane przez baze danych
                    // bo po zapisie danych w bazie danych przez EF
                    // ten id => user.Id, ktory jest kluczem glownym naszej encji
                    // zostanie automatycznie przez Entity Framework wypelniony takim id
                    // jakie nada baza danych

                    // czyli dostaniemy id nowoutworzonego uzytkownika
                    UserId = user.Id, 
                };
            }
        }

        // dodajemy to jako osobna klasa
        // on dziediczy po AbstractValidatorze to jest klasa z fluentvalidation
        // wszystkie klasy dziedziczace po AbstractValidator sa rejestrowane automatycznie w 
        // kontenerze DI dzieki metodzie AddValidatorsFromAssemblyContaining()
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                // Dodaj reguły walidacji
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Email).EmailAddress();
                RuleFor(x => x.Email).MaximumLength(100);

                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Password).MaximumLength(50);
            }
        }

    }
}
