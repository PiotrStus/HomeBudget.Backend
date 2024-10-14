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

namespace HomeBudget.Application.Logic.EventHandlers.TransactionsChanges
{
    public class SendNotificationsHandler : BaseEventHandler, INotificationHandler<TransactionCreatedEvent>, INotificationHandler<TransactionUpdatedEvent>
    {
        private readonly CategoryFilledLevelExceededChecker _categoryFilledLevelExceededChecker;
        private readonly CategoryExceededSender _categoryExceededSender;

        public SendNotificationsHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevelExceededChecker categoryFilledLevelExceededChecker, CategoryExceededSender categoryExceededSender) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevelExceededChecker = categoryFilledLevelExceededChecker;
            _categoryExceededSender = categoryExceededSender;
        }

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            await HandleTransaction(notification.TransactionId, notification.CategoryId, cancellationToken);
        }

        public async Task Handle(TransactionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await HandleTransaction(notification.TransactionId, notification.CategoryId, cancellationToken);
        }

        private async Task HandleTransaction(int transactionId, int categoryId, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            if (account == null)
            {
                throw new UnauthorizedException();
            }

            var limitExceeded = await _categoryFilledLevelExceededChecker
                .IsCategoryBudgetExceededOnTransactionChange(transactionId, account.Id, cancellationToken);

            if (limitExceeded == true)
            {
                await _categoryExceededSender.SendNotification(account.Id, categoryId, "CategoryLimitExceeded", cancellationToken);
            }
        }
    }
}
