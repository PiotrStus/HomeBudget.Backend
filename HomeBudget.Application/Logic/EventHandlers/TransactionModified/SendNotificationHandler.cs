using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Application.Exceptions;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Application.Services;
using HomeBudget.Application.Logic.Events;

namespace HomeBudget.Application.Logic.EventHandlers.TransactionCreated
{
    public class CalculateTransactionsHandler2 : BaseEventHandler, INotificationHandler<TransactionCreatedEvent>
    {
        private readonly CategoryFilledLevelExceededChecker _categoryFilledLevelExceededChecker;
        private readonly CategoryExceededSender _categoryExceededSender;

        public CalculateTransactionsHandler2(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevelExceededChecker categoryFilledLevelExceededChecker, CategoryExceededSender categoryExceededSender) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevelExceededChecker = categoryFilledLevelExceededChecker;
            _categoryExceededSender = categoryExceededSender;
        }

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            var transactionData = new TransactionData()
            {
                TransactionId = notification.TransactionId,
                CategoryId = notification.CategoryId,
                AccountId = account.Id,
                Amount = notification.Amount,
                Date = notification.Date
            };

            var limitExceeded = await _categoryFilledLevelExceededChecker.IsCategoryBudgetExceededOnTransactionChange(transactionData, cancellationToken);

            if (limitExceeded == true)
            {
                await _categoryExceededSender.SendNotification(transactionData.AccountId, transactionData.CategoryId, "CategoryLimitExceeded", cancellationToken);
            }
        }
    }
}
