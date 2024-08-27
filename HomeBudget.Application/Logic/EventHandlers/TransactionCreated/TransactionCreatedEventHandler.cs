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

namespace HomeBudget.Application.Logic.EventHandlers.TransactionCreated
{
    public class TransactionCreatedEventHandler : BaseEventHandler, INotificationHandler<TransactionCreatedEvent>
    {
        private readonly CategoryFilledLevel _categoryFilledLevel;
        private readonly CategoryFilledLevelExceededChecker _categoryFilledLevelExceededChecker;

        public TransactionCreatedEventHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevel categoryFilledLevel, CategoryFilledLevelExceededChecker categoryFilledLevelExceededChecker) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevel = categoryFilledLevel;
            _categoryFilledLevelExceededChecker = categoryFilledLevelExceededChecker;
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

            await _categoryFilledLevel.UpdateCategoryFilledLevel(transactionData, cancellationToken);

            var limitExceeded = await _categoryFilledLevelExceededChecker.IsCategoryBudgetExceeded(transactionData, cancellationToken);
        }
    }
}
