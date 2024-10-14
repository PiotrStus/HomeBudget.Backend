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
using static HomeBudget.Application.Logic.Budget.GetBudgetsQuery.Result;

namespace HomeBudget.Application.Logic.Budget
{
    public static class GetBudgetsQuery
    {

        public class Request() : IRequest<Result>
        {

        }

        public class Result()
        {
            public required List<YearBudget> YearBudgets { get; set; } = new List<YearBudget>();

            public class MonthlyBudget()
            {
                public required int Id { get; set; }

                public required Month Month { get; set; }

                public required decimal TotalAmount { get; set; }

            }

            public class YearBudget()
            {
                public required int Id { get; set; }

                public required int Year { get; set; }

                public string? Description { get; set; }

                public List<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();
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

                var yearBudgets = await _applicationDbContext.YearBudgets
                     .Where(y => y.AccountId == account.Id)
                     .Select(y => new Result.YearBudget()
                     {
                         Id = y.Id,
                         Year = y.Year,
                         Description = y.Description,
                         MonthlyBudgets = y.MonthlyBudgets
                            .Select(mb => new Result.MonthlyBudget()
                            {
                                Id = mb.Id,
                                Month = mb.Month,
                                TotalAmount = mb.TotalAmount
                            })
                            .ToList()
                     })
                     .ToListAsync(cancellationToken);




                return new Result()
                {
                    YearBudgets = yearBudgets
                };
            }
        }
    }
}
