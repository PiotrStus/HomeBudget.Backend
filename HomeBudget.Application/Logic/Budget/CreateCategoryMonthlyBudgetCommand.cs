using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities.Budget.Budget;
using HomeBudget.Domain.Entities.Budget;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Application.Exceptions;

namespace HomeBudget.Application.Logic.Budget
{
    public static class CreateCategoryMonthlyBudgetCommand
    {
        public class Request : IRequest<Result>
        {
            public int CategoryId { get; set; }

            public int MonthlyBudgetId { get; set; }

            public required decimal Amount { get; set; } = default!;
        }

        public class Result 
        {
            public int MonthlyBudgetCategoryId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {

            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var categoryMonthlyBudget = new MonthlyBudgetCategory()
                {
                    Amount = request.Amount,
                    CategoryId = request.CategoryId,
                    MonthlyBudgetId = request.MonthlyBudgetId
                };

                _applicationDbContext.MonthlyBudgetCategories.Add(categoryMonthlyBudget);

                await _applicationDbContext.SaveChangesAsync();

                return new Result
                {
                    MonthlyBudgetCategoryId = categoryMonthlyBudget.CategoryId,
                };
            }
        }
    }
}
