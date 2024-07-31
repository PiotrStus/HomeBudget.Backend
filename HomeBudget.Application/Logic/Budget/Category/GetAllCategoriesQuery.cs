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

namespace HomeBudget.Application.Logic.Budget.Category
{
    public static class GetAllCategoriesQuery
    {

        public class Request() : IRequest<Result>
        {

        }

        public class Result()
        {
            public required List<AccountCategories> Categories { get; set; } = new List<AccountCategories>();

            public class AccountCategories()
            {
                public required int Id { get; set; }

                public required string Name { get; set; }

                public required CategoryType CategoryType { get; set; }

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

                var categories = await _applicationDbContext.Categories
                     .Where(c => c.AccountId == account.Id && c.IsDeleted == false)
                     .Select(c => new Result.AccountCategories()
                     {
                         Id = c.Id,
                         Name = c.Name,
                         CategoryType = c.CategoryType,
                     })
                     .ToListAsync();




                return new Result()
                {
                    Categories = categories
                };
            }
        }
    }
}
