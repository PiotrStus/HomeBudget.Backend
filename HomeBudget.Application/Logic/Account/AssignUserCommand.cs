using FluentValidation;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using BudgetEntities = HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Domain.Entities;

namespace HomeBudget.Application.Logic.Budget.Account
{
    public static class AssignUserCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Email { get; set; }
        }

        public class Result
        {
            public int UserId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                if (account == null)
                {
                    throw new UnauthorizedException();
                }

                var accountUserExist = await _applicationDbContext.AccountUsers
                                                    .Where(au => au.User.Email == request.Email && au.AccountId == account.Id)
                                                    .AnyAsync(cancellationToken);

                if (accountUserExist)
                {
                    throw new ErrorException("UserWithThisEmailAlreadyAssigned");
                }

                var user = await _applicationDbContext.Users
                                                    .Where(u => u.Email == request.Email)
                                                    .FirstOrDefaultAsync(cancellationToken);

                if (user == null)
                {
                    throw new ErrorException("UserWithThisEmailNotExist");
                }

                var accountUser = new AccountUser
                {
                    User = user,
                    Account = account
                };

                _applicationDbContext.AccountUsers.Add(accountUser);


                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                {
                    UserId = accountUser.UserId
                };
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.Email).NotEmpty();
                    RuleFor(x => x.Email).MaximumLength(100);
                    RuleFor(x => x.Email).EmailAddress();
                }
            }
        }
    }
}
