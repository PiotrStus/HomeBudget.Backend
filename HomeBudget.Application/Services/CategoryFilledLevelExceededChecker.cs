using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeBudget.Application.Services
{
    public class CategoryFilledLevelExceededChecker
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ILogger<CategoryFilledLevelExceededChecker> _logger;

        public CategoryFilledLevelExceededChecker(IApplicationDbContext applicationDbContext, ILogger<CategoryFilledLevelExceededChecker> logger)
        {
            _applicationDbContext = applicationDbContext;
            _logger = logger;
        }

        public async Task<bool?> IsCategoryBudgetExceededOnTransactionChange(int transactionId, int accountId, CancellationToken cancellationToken)
        {
            var transaction = await _applicationDbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);
            if (transaction != null)
            {
                var year = transaction.Date.Year;
                var monthNumber = transaction.Date.Month;
                var categoryId = transaction.CategoryId;

                var query = _applicationDbContext.MonthlyBudgetCategories
                    //.Include(mc => mc.MonthlyBudgetCategoryTracking)
                    .Where(mc => mc.MonthlyBudget.YearBudget.Year == year)
                    .Where(mc => mc.MonthlyBudget.Month == (Month)monthNumber)
                    .Where(mc => mc.MonthlyBudget.YearBudget.AccountId == accountId)
                    .Where(mc => mc.CategoryId == categoryId)
                    .Select(mc => new
                    {
                        Tracking = mc.MonthlyBudgetCategoryTracking,
                        mc.Amount,
                        MonthlyBudgetCategoryId = mc.Id
                    });

                var monthlyBudgetCategory = await query.FirstOrDefaultAsync(cancellationToken);

                if (monthlyBudgetCategory == null)
                {
                    return null;
                }


                var monthlyLimit = monthlyBudgetCategory.Amount;

                var currentTransactionsTotalAmount = await _applicationDbContext.Transactions
                    .Where(t => t.Id != transactionId && t.CategoryId == categoryId && t.AccountId == accountId && t.Date.Year == year && t.Date.Month == monthNumber && !t.IsDeleted)
                    .SumAsync(t => t.Amount);

                if (currentTransactionsTotalAmount > monthlyLimit)
                {
                    return false;
                }

                currentTransactionsTotalAmount += transaction.Amount;

                _logger.LogCritical($"currentTransactionsTotalAmount: {currentTransactionsTotalAmount} > monthlyBudgetCategory.Amount: {monthlyBudgetCategory.Amount}");
                return currentTransactionsTotalAmount > monthlyLimit;
            }
            return null;
        }


        public async Task<bool?> IsCategoryBudgetExceededOnCategoryAmountChange(int plannedCategoryId, int accountId, CancellationToken cancellationToken)
        {
            var plannedCategory = await _applicationDbContext.MonthlyBudgetCategories
                                .Where(m => m.Id == plannedCategoryId && m.MonthlyBudget.YearBudget.Account.Id == accountId)
                                .Select(m => new
                                {
                                    m.MonthlyBudget.Month,
                                    m.Amount,
                                    m.CategoryId,
                                    m.MonthlyBudget.YearBudget.Year,
                                })
                                .FirstOrDefaultAsync(cancellationToken);

            if (plannedCategory != null)
            {

                var currentTransactionsTotalAmount = await _applicationDbContext.Transactions
                    .Where(t => t.CategoryId == plannedCategory.CategoryId
                                && t.AccountId == accountId
                                && t.Date.Year == plannedCategory.Year
                                && t.Date.Month == (int)plannedCategory.Month
                                && !t.IsDeleted)
                    .SumAsync(t => t.Amount, cancellationToken);


                return currentTransactionsTotalAmount > plannedCategory.Amount;
            }

            return null;
        }
    }
}
