﻿using FluentValidation;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using BudgetEntities = HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Domain.Entities.Budget;

namespace HomeBudget.Application.Logic.Budget.PlannedCategory
{
    public static class CreatePlannedCategoryCommand
    {
        public class Request : IRequest<Result>
        {
            public required decimal TotalAmount { get; set; }

            public required int MonthlyBudgetId { get; set; }

            public required int CategoryId { get; set; }
        }

        public class Result
        {
            public int PlannedCategoryId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
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

                var category = await _applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId && c.AccountId == account.Id && !c.IsDeleted);

                if (category == null)
                {
                    throw new ErrorException("CategoryNotFound");
                }

                var monthlyBudget = await _applicationDbContext.MonthlyBudgets.FirstOrDefaultAsync(m => m.Id == request.MonthlyBudgetId && m.YearBudget.AccountId == account.Id);

                if (monthlyBudget == null)
                {
                    throw new ErrorException("MonthlyBudgetNotFound");
                }

                var plannedCategoryAlreadyExist = await _applicationDbContext.MonthlyBudgetCategories.AnyAsync(m => m.MonthlyBudgetId == request.MonthlyBudgetId && m.Category.Id == request.CategoryId);

                if (plannedCategoryAlreadyExist)
                {
                    throw new ErrorException("PlannedCategoryAlreadyExists");
                }


                //var monthlyBudgetCategoryExist = await _applicationDbContext.MonthlyBudgetCategories
                //    .AnyAsync(m => m.);


                var plannedCategory = new MonthlyBudgetCategory()
                {
                    Amount = request.TotalAmount,
                    MonthlyBudget = monthlyBudget,
                    Category = category
                };
                monthlyBudget.MonthlyBudgetCategories.Add(plannedCategory);


                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                {
                    PlannedCategoryId = plannedCategory.Id
                };
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                }
            }
        }
    }
}
