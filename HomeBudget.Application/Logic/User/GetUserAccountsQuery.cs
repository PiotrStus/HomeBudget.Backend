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
using HomeBudget.Domain.Enums;

namespace HomeBudget.Application.Logic.User
{
    public static class GetUserAccountsQuery
    {
        public class Request : IRequest<Result>
        {

        }

        public class Result
        {
            public required List<UserAccount> Accounts { get; set; } = new List<UserAccount>();

            public class UserAccount()
            {
                public required int Id { get; set; }

                public required string Name { get; set; }
            }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IUserAccountsProvider _userAccountsProvider;

            public Handler(ICurrentAccountProvider currentAccountProvider,
                IApplicationDbContext applicationDbContext,
                IUserAccountsProvider userAccountsProvider
                ) : base(currentAccountProvider, applicationDbContext)
            {
                _userAccountsProvider = userAccountsProvider;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var accounts = await _userAccountsProvider.GetUserAccounts();

                var userAccounts = accounts.Select(account => new Result.UserAccount
                {
                    Id = account.Id,
                    Name = account.Name
                }).ToList();

                return new Result()
                {
                    Accounts = userAccounts
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