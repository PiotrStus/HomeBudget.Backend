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

namespace HomeBudget.Application.Logic.EventHandlers.PlannedCategoryAmountChanged
{
    public class SendNotificationHandler : BaseEventHandler, INotificationHandler<PlannedCategoryAmountChangedEvent>
    {
        private readonly CategoryFilledLevelExceededChecker _categoryFilledLevelExceededChecker;
        private readonly CategoryExceededSender _categoryExceededSender;

        public SendNotificationHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevelExceededChecker categoryFilledLevelExceededChecker, CategoryExceededSender categoryExceededSender) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevelExceededChecker = categoryFilledLevelExceededChecker;
            _categoryExceededSender = categoryExceededSender;
        }

        public async Task Handle(PlannedCategoryAmountChangedEvent notification, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            if (account == null)
            {
                throw new UnauthorizedException();
            }

            var limitExceeded = await _categoryFilledLevelExceededChecker.IsCategoryBudgetExceededOnCategoryAmountChange(notification.Id, account.Id, cancellationToken);
            
            if (limitExceeded == true)
            {
                await _categoryExceededSender.SendNotification(account.Id, notification.CategoryId, "CategoryLimitExceeded", cancellationToken);
            }
        }
    }
}
