using EasyCaching.Core.Interceptor;
using EFCoreSecondLevelCacheInterceptor;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Budget.Account
{
    public static class GetUsersQuery
    {

        public class Request() : IRequest<Result>
        {

        }

        public class Result()
        {
            public required List<User> Users { get; set; } = new List<User>();

            public class User()
            {
                public required int Id { get; set; }

                public required string Email { get; set; }

                public required bool isAdmin { get; set; }

            }
        }

        public class Handler : BaseQueryHandler, IRequestHandler<Request, Result>
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
                var users = await _applicationDbContext.AccountUsers
                     .Where(u => u.AccountId == account.Id)
                     .Select(c => new Result.User()
                     {
                         Id = c.UserId,
                         Email = c.User.Email,
                         isAdmin = c.IsAdmin,
                     })
                     .Cacheable()
                     .ToListAsync();

                return new Result()
                {
                    Users = users
                };
            }
        }
    }
}
