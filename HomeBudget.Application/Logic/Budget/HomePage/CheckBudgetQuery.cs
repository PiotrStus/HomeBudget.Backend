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

public static class CheckBudgetQuery
{

    public class Request() : IRequest<Result>
    {
        public required DateTimeOffset Date { get; set; }

    }

    public class Result()
    {
        public required bool BudgetExist { get; set; }
    }

    public class Handler : BaseQueryHandler, IRequestHandler<Request, Result>
    {
        public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
        {
        }

        public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            var monthFromRequest = (Month)request.Date.Month;

            var yearFromRequest = request.Date.Year;

            var monthlyBudgetExist = await _applicationDbContext.MonthlyBudgetCategories
                .Where(p => p.MonthlyBudget.Month == monthFromRequest && 
                       p.MonthlyBudget.YearBudget.Year == yearFromRequest &&
                       p.MonthlyBudget.YearBudget.AccountId == account.Id)
                .Cacheable()
                .AnyAsync(cancellationToken);

            return new Result()
            {
                BudgetExist = monthlyBudgetExist
            };
        }
    }
}