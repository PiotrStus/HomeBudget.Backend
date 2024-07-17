using EFCoreSecondLevelCacheInterceptor;
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
using static HomeBudget.Application.Logic.User.CreateUserWithAccountCommand.Request;

namespace HomeBudget.Application.Logic.User
{
    public static class LoggedInUserQuery
    {
        // parametry requesta beda puste
        public class Request : IRequest<Result>
        {

        }

        // natomiast to co bedziemy zwracac to aktualnie email uzytkownika
        // mozna zwracac inne
        public class Result
        {
            public required string Email { get; set; }
        }

        // zmieniamy klasę bazową z BaseCommandHandler na BaseQuerydHandler
        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IAuthenticationDataProvider _authenticationDataProvider;

            // do Handlera potrzebowac bedziemy wstrzyknac IAuthenticationDataProvider
            // zeby moc pobrac Id aktualnie zalogowanego uzytkownika
            public Handler(ICurrentAccountProvider currentAccountProvider,
                IApplicationDbContext applicationDbContext,
                IAuthenticationDataProvider authenticationDataProvider
                ) : base(currentAccountProvider, applicationDbContext)
            {
                _authenticationDataProvider = authenticationDataProvider;
            }

            // pierwsze co robimy to w handlerze pobieramy UserId aktalnie zalogowanego uzytkownika
            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                // pierwsze co robimy to w handlerze pobieramy UserId aktalnie zalogowanego uzytkownika
                var userId = _authenticationDataProvider.GetUserId();

                // jesli id ma wartosc
                if (userId.HasValue)
                {
                    // to wyciagamy z bazy danych z tabeli z uzytkownikami
                    // naszego uzytkownika o danym id
                    // mamy tutaj urzycie cache'a Cacheable() wiec te dane sie zcachuja
                    var user = await _applicationDbContext.Users.Cacheable().FirstOrDefaultAsync(u => u.Id == userId.Value);
                    // jesli istnieje w bazie to zwracamy jego email
                    // a jesli nie to tez wyrzucimy ten wyjatek
                    if (user != null)
                    {
                        return new Result()
                        {
                            Email = user.Email,
                        };
                    }
                }
                // jesli to id nie ma value czyli jest nullem no to wyrzucamy wyjatek
                throw new UnauthorizedException();
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                // walidator pozostaje pusty bo nie mamy zadnych parametrow requesta
            }
        }

    }
}
