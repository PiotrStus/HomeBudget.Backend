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





        public async Task<bool?> IsCategoryBudgetExceeded(TransactionData transactionData, CancellationToken cancellationToken)
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
                return null;
            }

            var monthlyBudgetCategory = await _applicationDbContext.MonthlyBudgetCategories
                .Include(mc => mc.MonthlyBudgetCategoryTracking)
                .FirstOrDefaultAsync(mc => mc.MonthlyBudgetId == monthlyBudget.Id && mc.CategoryId == transactionData.CategoryId);

            if (monthlyBudgetCategory == null)
            {
                return null;
            }

            var limit = monthlyBudgetCategory.Amount;

            var currentTransactionsTotalAmount = await _applicationDbContext.Transactions
                .Where(t => t.Id != transactionData.TransactionId && t.CategoryId == transactionData.CategoryId && t.AccountId == transactionData.AccountId && t.Date.Year == year && t.Date.Month == monthNumber)
                .SumAsync(t => t.Amount);

            currentTransactionsTotalAmount += transactionData.Amount;

            _logger.LogCritical($"currentTransactionsTotalAmount: {currentTransactionsTotalAmount} > monthlyBudgetCategory.Amount: {monthlyBudgetCategory.Amount}");
            return currentTransactionsTotalAmount > monthlyBudgetCategory.Amount;

        }
    }
}
