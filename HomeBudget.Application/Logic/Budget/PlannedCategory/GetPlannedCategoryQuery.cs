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
using static HomeBudget.Application.Logic.Budget.PlannedCategory.GetPlannedCategoriesQuery.Result;

namespace HomeBudget.Application.Logic.Budget.PlannedCategory
{
    public static class GetPlannedCategoryQuery
    {

        public class Request() : IRequest<Result>
        {
            public required int Id { get; set; }

        }

        public class Result()
        {
            public required decimal Amount { get; set; }

            public required Month Month { get; set; }

            public required string Category { get; set; }

            public required CategoryType CategoryType { get; set; }
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

                var plannedCategory = await _applicationDbContext.MonthlyBudgetCategories
                    .Include(c => c.Category)
                    .Include(mb => mb.MonthlyBudget)
                    .FirstOrDefaultAsync(m => m.Id == request.Id && m.MonthlyBudget.YearBudget.AccountId == account.Id);

                if (plannedCategory == null)
                {
                    throw new ErrorException("PlannedCategoryNotFound");
                }

                return new Result()
                {
                    Amount = plannedCategory.Amount,
                    Month = plannedCategory.MonthlyBudget.Month,
                    Category = plannedCategory.Category.Name,
                    CategoryType = plannedCategory.Category.CategoryType
                };
            }
        }
    }
}
