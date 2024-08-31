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





        public async Task<bool?> IsCategoryBudgetExceededOnTransactionChange(TransactionData transactionData, CancellationToken cancellationToken)
        {
            var year = transactionData.Date.Year;
            var monthNumber = transactionData.Date.Month;
            Month? month = null;

            if (Enum.IsDefined(typeof(Month), monthNumber))
            {
                month = (Month)monthNumber;
            }

            var monthlyBudget = await _applicationDbContext.MonthlyBudgets
                .FirstOrDefaultAsync(mb => mb.YearBudget.Year == year && mb.Month == month && mb.YearBudget.AccountId == transactionData.AccountId, cancellationToken);

            if (monthlyBudget == null)
            {
                return null;
            }

            var monthlyBudgetCategory = await _applicationDbContext.MonthlyBudgetCategories
                .Include(mc => mc.MonthlyBudgetCategoryTracking)
                .FirstOrDefaultAsync(mc => mc.MonthlyBudgetId == monthlyBudget.Id && mc.CategoryId == transactionData.CategoryId);

            if (monthlyBudgetCategory == null)
            {
                return null;
            }

            var monthlyLimit = monthlyBudgetCategory.Amount;

            var currentTransactionsTotalAmount = await _applicationDbContext.Transactions
                .Where(t => t.Id != transactionData.TransactionId && t.CategoryId == transactionData.CategoryId && t.AccountId == transactionData.AccountId && t.Date.Year == year && t.Date.Month == monthNumber)
                .SumAsync(t => t.Amount);

            if (currentTransactionsTotalAmount > monthlyLimit)
            {
                return false;
            }


            currentTransactionsTotalAmount += transactionData.Amount;

            _logger.LogCritical($"currentTransactionsTotalAmount: {currentTransactionsTotalAmount} > monthlyBudgetCategory.Amount: {monthlyBudgetCategory.Amount}");
            return currentTransactionsTotalAmount > monthlyLimit;

        }


        public async Task<bool?> IsCategoryBudgetExceededOnCategoryChange(int accountId, int plannedCategoryId, CancellationToken cancellationToken)
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
                                && t.Date.Month == (int) plannedCategory.Month)
                    .SumAsync(t => t.Amount, cancellationToken);

                return currentTransactionsTotalAmount > plannedCategory.Amount;
            }

            return null;
        }
    }
}
