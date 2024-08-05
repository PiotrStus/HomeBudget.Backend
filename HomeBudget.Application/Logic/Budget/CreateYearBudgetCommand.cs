﻿using FluentValidation;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Entities.Budget.Budget;
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
    public static class CreateYearBudgetCommand
    {
        public class Request : IRequest<Result>
        {
            public required int Year { get; set; }
            public string? Description { get; set; }

        }

        public class Result
        {
            public int YearBudgetId {  get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var yearBudgetExist = await _applicationDbContext.YearBudgets.AnyAsync(y => y.Year == request.Year && y.AccountId == account.Id);

                if (yearBudgetExist)
                {
                    throw new ErrorException("AccountWithThisYearBudgetAlreadyExists");
                }

                var categories = await _applicationDbContext.Categories
                    .Where(c => c.CategoryType == CategoryType.Expense)
                    .ToListAsync();

                using (var yearBudgetTransaction = await _applicationDbContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    var yearBudget = new YearBudget()
                    {
                        Year = request.Year,
                        Description = request.Description,
                        AccountId = account.Id
                    };

                    _applicationDbContext.YearBudgets.Add(yearBudget);

                    for (var i = 1; i <= 12; i++)
                    {
                        var monthlyBudget = new MonthlyBudget()
                        {
                            Month = (Month)i,
                            YearBudget = yearBudget,
                            TotalAmount = 0,
                        };
                        yearBudget.MonthlyBudgets.Add(monthlyBudget);


                        foreach (var category in categories)
                        {
                            if (category.CategoryType == CategoryType.Income && !category.IsDeleted )
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
                    }

                    await _applicationDbContext.SaveChangesAsync(cancellationToken);

                    await yearBudgetTransaction.CommitAsync(cancellationToken);

                    return new Result()
                    {
                        YearBudgetId = yearBudget.Id
                    };
                }
            }
        }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.Year).NotEqual(0);
                    RuleFor(x => x.Description).MaximumLength(255);
                }
            }
    }
}
