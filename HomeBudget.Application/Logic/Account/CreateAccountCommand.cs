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

namespace HomeBudget.Application.Logic.Account
{
    public static class CreateAccountCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Name { get; set; }
        }

        public class Result
        {
            public required int AccountId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IAuthenticationDataProvider _authenticationDataProvider;

            public Handler(IAuthenticationDataProvider authenticationDataProvider,ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
                _authenticationDataProvider = authenticationDataProvider;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = _authenticationDataProvider.GetUserId();

                var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new ErrorException("UserNotExist");
                }

                var utcNow = DateTime.UtcNow;

                var account = new Domain.Entities.Account()
                {
                    Name = request.Name,
                    CreateDate = utcNow,
                };

                _applicationDbContext.Accounts.Add(account);

                var accountUser = new AccountUser()
                {
                    Account = account,
                    User = user
                };

                _applicationDbContext.AccountUsers.Add(accountUser);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                { 
                    AccountId = account.Id, 
                };
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Name).MaximumLength(100);
            }
        }
    }
}