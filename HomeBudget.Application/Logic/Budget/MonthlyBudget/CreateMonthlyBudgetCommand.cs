using FluentValidation;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Application.Exceptions;
using BudgetMonthlyBudget3 = HomeBudget.Domain.Entities.Budget.MonthlyBudget;

namespace HomeBudget.Application.Logic.Budget.MonthlyBudget
{
    public static class CreateMonthlyBudgetCommand
    {
        public class Request : IRequest<Result>
        {
            public required int YearBudgetId { get; set; }

            public required Month Month { get; set; }

            public decimal? TotalAmount { get; set; }
        }

        public class Result
        {
            public int MonthlyBudgetId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var categories = await _applicationDbContext.Categories
                    .Where(c => c.IsDraft && c.AccountId == account.Id)
                    .ToListAsync();

                var monthlyBudgetExist = await _applicationDbContext.MonthlyBudgets
                    //.Include(m => m.YearBudget)
                    .AnyAsync(m => m.YearBudgetId == request.YearBudgetId && m.Month == request.Month && m.YearBudget.AccountId == account.Id);

                if (monthlyBudgetExist)
                {
                    throw new ErrorException("MonthlyBudgetAlreadyExists");
                }

                if (request.TotalAmount != null)
                {
                }

                var monthlyBudget = new HomeBudget.Domain.Entities.Budget.MonthlyBudget()
                {
                    YearBudgetId = request.YearBudgetId,
                    Month = request.Month,
                    TotalAmount = request.TotalAmount ?? 0m
                };

                foreach (var category in categories)
                {
                    //if (category.CategoryType == CategoryType.Expense && !category.IsDeleted )
                    if (!category.IsDeleted)
                    {
                        var plannedCategory = new MonthlyBudgetCategory()
                        {
                            Amount = 0,
                            MonthlyBudget = monthlyBudget,
                            Category = category
                        };
                        monthlyBudget.MonthlyBudgetCategories.Add(plannedCategory);
                    }
                }



                _applicationDbContext.MonthlyBudgets.Add(monthlyBudget);

                await _applicationDbContext.SaveChangesAsync();


                return new Result()
                {
                    MonthlyBudgetId = monthlyBudget.Id
                };
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.YearBudgetId).NotEmpty();
                    RuleFor(x => x.Month).IsInEnum();
                    RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
                }
            }
        }
    }
}
