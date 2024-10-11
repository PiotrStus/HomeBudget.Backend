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
    public static class LoginCommand
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
            // wlasciwosci requesta, uzytkownik bedzie podawac email i haslo
            public required string Email { get; set; }
            public required string Password { get; set; }

        }


        // Result zwraca dane, który zawiera dany command

        public class Result
        {
            // w rezultacie bedziemy zwracac userid
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
                // zostawiamy go i posluzy nam do weryfikacji hasla
                _passwordManager = passwordManager;
            }


            // tutaj obslugujemy logike tworzenia takiego konta
            // czyli jesli przyjdzie command z uzytkownikiem to tutaj musimy zawrzec 
            // cala logike tworzenia takiego konta
            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                // pobieramy uzytkownika z bazy danych na podstawie maila
                var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                
                if (user != null)
                {
                    // jesli user istnieje to uzywamy metody VerifyPassword
                    // i podajemy tutaj z bazdy danych zahasowane haslo i haslo wprowadzone
                    // przez uzytkownika w requestie
                    // w srodku te metoda porownuje hasla, wylicza hasha na nowo
                    if (_passwordManager.VerifyPassword(user.HashedPassword, request.Password))
                    {
                        // jesli wszystko sie zgadza to zwracamy id uzytkownika
                        return new Result()
                        {
                            UserId = user.Id
                        };
                    }
                }

                // jesli uzytkownik nie istnieje to wyrzucamy wyjatek InvalidLoginOrPassword
                // to jest kwestia bezpieczenstwa
                // bo jak sie wyrzuci uzytkownik nie istnieje to ktos odpytujac api z roznymi mailami
                // czy na tego maila zostalo konto czy nie i probowac zgadnac do niego haslo
                throw new ErrorException("InvalidLoginOrPassword");



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
                // reguły walidacji
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Email).EmailAddress();

                RuleFor(x => x.Password).NotEmpty();
            }
        }

    }
}
