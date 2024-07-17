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
    public static class LogoutCommand
    {

        // parametrow requesta nie potrzebujemy zadnych
        public class Request : IRequest<Result>
        {

        }

        // nie bedziemy tez nic zwracac
        public class Result
        {

        }


        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {

            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                // zostawiamy pusta implementacje
                // jakby byla potrzeba np. zapisania do logow,ze
                // uzytkownik sie wylogowal
                return new Result()
                {

                };
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {

            }
        }

    }
}
