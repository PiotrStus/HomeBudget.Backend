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


namespace HomeBudget.Application.Logic.Budget.HomePage;

public static class GetPlannedMonthlyCategoriesQuery
{

    public class Request() : IRequest<Result>
    {
        public required DateTimeOffset Date { get; set; }

    }

    public class Result()
    {
        public required List<PlannedMonthlyCategoriesBudgets> PlannedCategories { get; set; } = new List<PlannedMonthlyCategoriesBudgets>();

        public class PlannedMonthlyCategoriesBudgets()
        {
            public required int Id { get; set; }

            public required string? Name { get; set; }

            public required decimal Amount { get; set; }

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

            if (account == null)
            {
                throw new UnauthorizedException();
            }

            var monthFromRequest = (Month)request.Date.Month;

            var yearFromRequest = request.Date.Year;

            var plannedCategories = await _applicationDbContext.MonthlyBudgetCategories
                .Where(p => p.MonthlyBudget.Month == monthFromRequest && p.MonthlyBudget.YearBudget.Year == yearFromRequest && p.MonthlyBudget.YearBudget.AccountId == account.Id)
                .Select(p => new Result.PlannedMonthlyCategoriesBudgets()
                {
                    Id = p.Id,
                    Name = p.Category.Name,
                    Amount = p.Amount,
                    CategoryType = p.Category.CategoryType
                })
                .Cacheable()
                .ToListAsync(cancellationToken);




            return new Result()
            {
                PlannedCategories = plannedCategories
            };
        }
    }
}
