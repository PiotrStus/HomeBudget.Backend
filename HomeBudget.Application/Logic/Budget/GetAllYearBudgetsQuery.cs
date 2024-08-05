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

namespace HomeBudget.Application.Logic.Budget
{
    public static class GetAllYearBudgetsQuery
    {

        public class Request() : IRequest<Result>
        {

        }

        public class Result()
        {
            public required List<AllYearBudgets> YearBudgets { get; set; } = new List<AllYearBudgets>();

            public class AllYearBudgets()
            {
                public required int Id { get; set; }

                public required int Year { get; set; }

                public string? Description { get; set; }

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

                var yearBudgets = await _applicationDbContext.YearBudgets
                     .Where(c => c.AccountId == account.Id)
                     .Select(c => new Result.AllYearBudgets()
                     {
                         Id = c.Id,
                         Year = c.Year,
                         Description = c.Description,
                     })
                     .ToListAsync();




                return new Result()
                {
                    YearBudgets = yearBudgets
                };
            }
        }
    }
}
