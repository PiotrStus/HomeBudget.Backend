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
using HomeBudget.Domain.Enums;

namespace HomeBudget.Application.Logic.User
{
    public static class CreateUserCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class Result
        {
            public required int UserId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IPasswordManager _passwordManager;

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, IPasswordManager passwordManager) : base(currentAccountProvider, applicationDbContext)
            {
                _passwordManager = passwordManager;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var userExists = await _applicationDbContext.Users.AnyAsync(u => u.Email == request.Email);
                if (userExists)
                {
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
                    IsActivated = false,
                    // jako haslo podajemy ciag pusty,
                    // w pliku DomainEntity to pole HashedPassword jest typu required
                    // wiec i tak trzeba je podac, zeby utworzyc w ogole obiekt
                    // wiec narazie podajemy pusty
                    HashedPassword = "",
                };

                // natomiast za chwile przypisujemy tutaj, z tego ktore podal uzytkownik
                // wykorzystujac password managera
                user.HashedPassword = _passwordManager.HashPassword(request.Password);

                var userGuid = new UserConfirmGuid()
                {
                    User = user,
                    UserGuid = Guid.NewGuid(),
                    GuidType = UserGuidType.ConfirmAccount
                };

                // nastepnie dodajemy do contextu EF do kolekcji users danego uzytkownika
                // on tutaj sie jeszcze nie zapisal do bazy danych
                // jest narazie dodany tutaj do pamieci 
                // natomiast zapis nastapi pozniej 
                _applicationDbContext.Users.Add(user);
                _applicationDbContext.UserConfirmGuids.Add(userGuid);

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
