using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Application.Logic.Events;
using HomeBudget.Application.Services;
using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using MediatR;

namespace HomeBudget.Application.Logic.EventHandlers.TransactionsChanges
{
    public class CalculatePlannedMonthlyBudgetCategoryHandler : BaseEventHandler, INotificationHandler<TransactionCreatedEvent>, INotificationHandler<TransactionDeletedEvent>, INotificationHandler<TransactionUpdatedEvent>
    {
        private readonly CategoryFilledLevelUpdater _categoryFilledLevel;


        public CalculatePlannedMonthlyBudgetCategoryHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevelUpdater categoryFilledLevel) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevel = categoryFilledLevel;
        }

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            if (account == null)
            {
                throw new UnauthorizedException();
            }

            await _categoryFilledLevel.UpdateSumAfterTransactionAdded(notification.TransactionId, account.Id, cancellationToken);
        }

        public async Task Handle(TransactionDeletedEvent notification, CancellationToken cancellationToken)
        {

            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            if (account == null)
            {
                throw new UnauthorizedException();
            }

            await _categoryFilledLevel.UpdateSumAfterTransactionDeleted(notification.TransactionId, account.Id, cancellationToken);
        }

        public async Task Handle(TransactionUpdatedEvent notification, CancellationToken cancellationToken)
        {

            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            if (account == null)
            {
                throw new UnauthorizedException();
            }

            await _categoryFilledLevel.UpdateSumAfterTransactionUpdated(notification.TransactionId, account.Id, notification.PreviousDate, notification.PreviousCategoryId, cancellationToken);
        }
    }
}
