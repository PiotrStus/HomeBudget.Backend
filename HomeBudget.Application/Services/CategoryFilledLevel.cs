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

        public async Task UpdateSumAfterTransactionChanged(TransactionData transactionData, CancellationToken cancellationToken)
        {
            var year = transactionData.Date.Year;
            var monthNumber = transactionData.Date.Month;
            //Month? month = null;

            //if (Enum.IsDefined(typeof(Month), monthNumber))
            //{
            //    month = (Month) monthNumber;
            //}

            //var monthlyBudgetId = await _applicationDbContext.MonthlyBudgets
            //    .Where(mb => mb.YearBudget.Year == year && mb.Month == (Month) monthNumber && mb.YearBudget.AccountId == transactionData.AccountId)
            //    .Select(mb => mb.Id)
            //    .FirstOrDefaultAsync(cancellationToken);

            //if (monthlyBudget == null)
            //{
            //    return;
            //}


            var query = _applicationDbContext.MonthlyBudgetCategories
                //.AsQueryable()
                .Where(mc => mc.MonthlyBudget.YearBudget.Year == year)
                .Where(mc => mc.MonthlyBudget.Month == (Month)monthNumber)
                .Where(mc => mc.MonthlyBudget.YearBudget.AccountId == transactionData.AccountId)
                .Where(mc => mc.CategoryId == transactionData.CategoryId)
                .Select(mc => new
                {
                    Tracking = mc.MonthlyBudgetCategoryTracking,
                    mc.Amount,
                    MonthlyBudgetCategoryId = mc.Id
                });

            var monthlyBudgetCategory = await query
                .FirstOrDefaultAsync(cancellationToken);



            //var monthlyBudgetCategory = await _applicationDbContext.MonthlyBudgetCategories
            //    .Where(mc => mc.MonthlyBudget.YearBudget.Year == year 
            //                 && mc.MonthlyBudget.Month == (Month) monthNumber 
            //                 && mc.MonthlyBudget.YearBudget.AccountId == transactionData.AccountId 
            //                 && mc.CategoryId == transactionData.CategoryId)
            //    .Select(mc => new
            //    {
            //        Tracking = mc.MonthlyBudgetCategoryTracking,
            //        mc.Amount,
            //        MonthlyBudgetCategoryId = mc.Id
            //    })
            //    .FirstOrDefaultAsync(cancellationToken);

            if (monthlyBudgetCategory == null)
            {
                return;
            }

            var currentTransactionsTotalAmount = await _applicationDbContext.Transactions
                .Where(t => t.CategoryId == transactionData.CategoryId
                            && t.AccountId == transactionData.AccountId
                            && t.Date.Year == year
                            && t.Date.Month == monthNumber
                            && !t.IsDeleted)
                .SumAsync(t => t.Amount, cancellationToken);

            monthlyBudgetCategory.Tracking.TransactionSum = currentTransactionsTotalAmount;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }


        public async Task UpdateSumAfterTransactionChanged(int transactionId, int accountId, CancellationToken cancellationToken)
        {
            var transaction = await _applicationDbContext.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction != null)
            {
                var year = transaction.Date.Year;
                var monthNumber = transaction.Date.Month;


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
                    .Where(t => t.CategoryId == transaction.CategoryId
                                && t.AccountId == transaction.AccountId
                                && t.Date.Year == year
                                && t.Date.Month == monthNumber
                                && (!t.IsDeleted || t.Id == transactionId))
                    .SumAsync(t => t.Amount, cancellationToken);


                    monthlyBudgetCategory.Tracking.TransactionSum = currentTransactionsTotalAmount - transaction.Amount;
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                    scope.Complete();
                }
            }
        }
    }
}