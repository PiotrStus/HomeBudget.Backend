﻿using HomeBudget.Application.Exceptions;
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
using static HomeBudget.Application.Logic.Budget.Category.GetPlannedCategoriesQuery.Result;

namespace HomeBudget.Application.Logic.Budget.Category
{
    public static class GetPlannedCategoriesQuery
    {

        public class Request() : IRequest<Result>
        {
            public required int YearId { get; set; }

            public required Month Month { get; set; }
        }

        public class Result()
        {
            public required List<PlannedCategoriesBudgets> PlannedCategories { get; set; } = new List<PlannedCategoriesBudgets>();

            public class PlannedCategoriesBudgets()
            {
                public required int Id { get; set; }

                public required string? Category { get; set; }

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

                var yearBudget = await _applicationDbContext.YearBudgets
                     .FirstOrDefaultAsync(y => y.AccountId == account.Id && y.Id == request.YearId, cancellationToken);

                if (yearBudget == null)
                {
                    throw new ErrorException("YearBudgetNotExists");

                }

                var monthlyBudget = await _applicationDbContext.MonthlyBudgets
                    .FirstOrDefaultAsync(m => m.Month == request.Month && m.YearBudget == yearBudget, cancellationToken);

                if (monthlyBudget == null)
                {
                    throw new ErrorException("MonthlyBudgetNotExists");
                }

                var plannedCategories = await _applicationDbContext.MonthlyBudgetCategories
                    .Where(p => p.MonthlyBudget == monthlyBudget)
                    .Select(p => new PlannedCategoriesBudgets()
                    {
                        Id = p.Id,
                        Category = p.Category.Name,
                        Amount = p.Amount,
                        CategoryType = p.Category.CategoryType
                    })
                    .ToListAsync(cancellationToken);




                return new Result()
                {
                    PlannedCategories = plannedCategories
                };
            }
        }
    }
}
