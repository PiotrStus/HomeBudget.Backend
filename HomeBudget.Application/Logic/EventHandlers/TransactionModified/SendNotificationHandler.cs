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
    public class SendNotificationsHandler : BaseEventHandler, INotificationHandler<ITransactionEvent>
    {
        private readonly CategoryFilledLevelExceededChecker _categoryFilledLevelExceededChecker;
        private readonly CategoryExceededSender _categoryExceededSender;

        public SendNotificationsHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevelExceededChecker categoryFilledLevelExceededChecker, CategoryExceededSender categoryExceededSender) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevelExceededChecker = categoryFilledLevelExceededChecker;
            _categoryExceededSender = categoryExceededSender;
        }

        public async Task Handle(ITransactionEvent notification, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            var limitExceeded = await _categoryFilledLevelExceededChecker.IsCategoryBudgetExceededOnTransactionChange(notification.TransactionId, account.Id, cancellationToken);

            if (limitExceeded == true)
            {
                await _categoryExceededSender.SendNotification(account.Id, notification.CategoryId, "CategoryLimitExceeded", cancellationToken);
            }
        }

        //public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        //{
        //    var account = await _currentAccountProvider.GetAuthenticatedAccount();

        //    var limitExceeded = await _categoryFilledLevelExceededChecker.IsCategoryBudgetExceededOnTransactionChange(notification.TransactionId, account.Id, cancellationToken);

        //    if (limitExceeded == true)
        //    {
        //        await _categoryExceededSender.SendNotification(notification.TransactionId, notification.CategoryId, "CategoryLimitExceeded", cancellationToken);
        //    }
        //}


        //public async Task Handle(TransactionUpdatedEvent notification, CancellationToken cancellationToken)
        //{
        //    var account = await _currentAccountProvider.GetAuthenticatedAccount();

        //    var limitExceeded = await _categoryFilledLevelExceededChecker.IsCategoryBudgetExceededOnTransactionChange(notification.TransactionId, account.Id, cancellationToken);

        //    if (limitExceeded == true)
        //    {
        //        await _categoryExceededSender.SendNotification(notification.TransactionId, notification.CategoryId, "CategoryLimitExceeded", cancellationToken);
        //    }
        //}
    }
}
