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

namespace HomeBudget.Application.Logic.Account
{
    public static class CurrentAccountQuery
    {
        // parametry requesta beda puste
        public class Request : IRequest<Result>
        {

        }

        // natomiast to co bedziemy zwracac to aktualnie name konta
        // mozna zwracac inne
        public class Result
        {
            public required string Name { get; set; }
        }

        // zmieniamy klasę bazową z BaseCommandHandler na BaseQuerydHandler
        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider,
                IApplicationDbContext applicationDbContext
                ) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                // GetAuthenticatedAccount - metoda ICurrentAccountProvidera, ktora juz implementowalismy wczesniej
                // odczytuje aktualne konto z ciastka no i jak nie istnieje to rzuca wyjatki
                // a jak istnieje to wyciaga sobie z bazy i zwraca cale to konto
                var account = await _currentAccountProvider.GetAuthenticatedAccount();
                return new Result()
                {
                    Name = account.Name
                };
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
