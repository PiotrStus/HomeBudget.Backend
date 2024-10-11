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
using static HomeBudget.Application.Logic.User.CreateUserCommand.Request;
using AccountEntity = HomeBudget.Domain.Entities.Account;


namespace HomeBudget.Application.Logic.Account
{
    public static class SwitchAccountQuery
    {
        public class Request : IRequest<Result>
        {
            public required int AccountId { get; set; }
        }

        public class Result
        {
            public int? Id { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IAuthenticationDataProvider _authenticationDataProvider;

            public Handler(IAuthenticationDataProvider authenticationDataProvider, ICurrentAccountProvider currentAccountProvider,
                IApplicationDbContext applicationDbContext
                ) : base(currentAccountProvider, applicationDbContext)
            {
                _authenticationDataProvider = authenticationDataProvider;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = _authenticationDataProvider.GetUserId();

                var userAccount = await _applicationDbContext.AccountUsers
                    .Where(au => au.UserId == userId && au.AccountId == request.AccountId)
                    .Select(au => new
                        {
                            AccountName = au.Account.Name,
                            au.AccountId
                        })
                    .FirstOrDefaultAsync();

                if (userAccount == null)
                {
                    throw new UnauthorizedException();
                }

                return new Result()
                {
                    Id = userAccount.AccountId
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
