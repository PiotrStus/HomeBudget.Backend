using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Domain.Entities;
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
using System.Transactions;

namespace HomeBudget.Application.Services
{
    public class CategoryFilledLevel
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public CategoryFilledLevel(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        
        public async Task UpdateMonthlyBudgetCategoryAfterTransactionChanged(int transactionId, int accountId, Func<decimal, decimal, decimal> updateFunction, CancellationToken cancellationToken, DateTimeOffset? previousDate = null, int? previousCategoryId = null)
        {
            var transaction = await _applicationDbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction != null)
            {
                var year = previousDate?.Year ?? transaction.Date.Year;
                var monthNumber = previousDate?.Month ?? transaction.Date.Month;
                var categoryId = previousCategoryId ?? transaction.CategoryId;


                var query = _applicationDbContext.MonthlyBudgetCategories
                    //.Include(mc => mc.MonthlyBudgetCategoryTracking)
                    .Where(mc => mc.MonthlyBudget.YearBudget.Year == year)
                    .Where(mc => mc.MonthlyBudget.Month == (Month)monthNumber)
                    .Where(mc => mc.MonthlyBudget.YearBudget.AccountId == accountId)
                    .Where(mc => mc.CategoryId == transaction.CategoryId)
                    .Select(mc => new
                    {
                        Tracking = mc.MonthlyBudgetCategoryTracking,
                        mc.Amount,
                        MonthlyBudgetCategoryId = mc.Id
                    });

                var monthlyBudgetCategory = await query.FirstOrDefaultAsync(cancellationToken);

                if (monthlyBudgetCategory == null)
                {
                    return;
                }

                using (var scope = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.RepeatableRead },
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    var currentTransactionsTotalAmount = await _applicationDbContext.Transactions
                    .Where(t => t.CategoryId == categoryId
                                && t.AccountId == transaction.AccountId
                                && t.Date.Year == year
                                && t.Date.Month == monthNumber
                                && (!t.IsDeleted || t.Id == transactionId))
                    .SumAsync(t => t.Amount, cancellationToken);


                    //monthlyBudgetCategory.Tracking.TransactionSum = currentTransactionsTotalAmount - transaction.Amount;
                    monthlyBudgetCategory.Tracking.TransactionSum = updateFunction(currentTransactionsTotalAmount, transaction.Amount);
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                    scope.Complete();
                }
            }
        }



        public async Task UpdateSumAfterTransactionAdded(int transactionId, int accountId, CancellationToken cancellationToken)
        {
            await UpdateMonthlyBudgetCategoryAfterTransactionChanged(transactionId, accountId,
                (currentTotal, transactionAmount) => currentTotal, cancellationToken);
        }

        public async Task UpdateSumAfterTransactionDeleted(int transactionId, int accountId, CancellationToken cancellationToken)
        {
            await UpdateMonthlyBudgetCategoryAfterTransactionChanged(transactionId, accountId,
                (currentTotal, transactionAmount) => currentTotal - transactionAmount, cancellationToken);
        }
    }
}