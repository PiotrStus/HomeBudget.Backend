using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Enums;
using HomeBudget.Domain.Entities.Budget;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HomeBudget.Application.Logic.Budget.Category.GetCategoryQuery.Result;
using static HomeBudget.Application.Logic.Budget.GetBudgetsQuery.Result;

namespace HomeBudget.Application.Logic.Budget.MonthlyBudget
{
    public static class GetMonthlyBudgetQuery
    {
        public class Request : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Result
        {
            public required Month Month { get; set; }

            public required decimal TotalAmount { get; set; }

            public required YearDTO Year { get; set; }


            public class YearDTO
            {
                public int Id { get; set; }

                public int Year { get; set; }
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

                var model = await _applicationDbContext.MonthlyBudgets
                    //.Include(mb => mb.YearBudget) -> eager loading
                    .Where(c => c.Id == request.Id && c.YearBudget.AccountId == account.Id)
                    .Select(mc => new { mc.Month, mc.YearBudget.Year, mc.YearBudgetId, mc.TotalAmount })
                    .FirstOrDefaultAsync();



                if (model == null)
                {
                    throw new UnauthorizedException();
                }


                return new Result
                {
                    Month = model.Month,
                    TotalAmount = model.TotalAmount,
                    Year = new Result.YearDTO()
                    {
                        Id = model.YearBudgetId,
                        Year = model.Year
                    }
                };
            }
        }
    }
}
