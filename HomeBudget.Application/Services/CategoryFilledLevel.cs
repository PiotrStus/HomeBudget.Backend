using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeBudget.Application.Services
{
    public class CategoryFilledLevel
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public CategoryFilledLevel(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task UpdateCategoryFilledLevel(TransactionData transactionData, CancellationToken cancellationToken)
        {
            var year = transactionData.Date.Year;
            var monthNumber = transactionData.Date.Month;
            Month? month = null;

            if (Enum.IsDefined(typeof(Month), monthNumber))
            {
                month = (Month) monthNumber;
            }

            var monthlyBudget = await _applicationDbContext.MonthlyBudgets
                .FirstOrDefaultAsync(mb => mb.YearBudget.Year == year && mb.Month == month && mb.YearBudget.AccountId == transactionData.AccountId, cancellationToken);

            if (monthlyBudget == null)
            {
                return;
            }

            var monthlyBudgetCategory = await _applicationDbContext.MonthlyBudgetCategories
                .Include(mc => mc.MonthlyBudgetCategoryTracking)
                .FirstOrDefaultAsync(mc => mc.MonthlyBudgetId == monthlyBudget.Id && mc.CategoryId == transactionData.CategoryId);

            if (monthlyBudgetCategory == null)
            {
                return;
            }

            var currentTransactionsTotalAmount = await _applicationDbContext.Transactions
                .Where(t => t.CategoryId == transactionData.CategoryId
                            && t.AccountId == transactionData.AccountId
                            && t.Date.Year == year
                            && t.Date.Month == monthNumber)
                .SumAsync(t => t.Amount, cancellationToken);

  

            var tracking = monthlyBudgetCategory.MonthlyBudgetCategoryTracking;

            if (tracking != null && monthlyBudgetCategory.Amount > 0)
            {
                var level = currentTransactionsTotalAmount / monthlyBudgetCategory.Amount * 100;
                tracking.CategoryFilledLevel = (int) level;
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }


        }
    }
}
