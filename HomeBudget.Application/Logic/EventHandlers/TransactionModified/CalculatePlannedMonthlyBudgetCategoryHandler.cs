using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Application.Logic.Events;
using HomeBudget.Application.Services;
using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using MediatR;

namespace HomeBudget.Application.Logic.EventHandlers.TransactionCreated
{
    public class CalculatePlannedMonthlyBudgetCategoryHandler : BaseEventHandler, INotificationHandler<TransactionCreatedEvent>, INotificationHandler<TransactionDeletedEvent>
    {
        private readonly CategoryFilledLevel _categoryFilledLevel;


        public CalculatePlannedMonthlyBudgetCategoryHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevel categoryFilledLevel) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevel = categoryFilledLevel;
        }

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            await _categoryFilledLevel.UpdateSumAfterTransactionAdded(notification.TransactionId, account.Id, cancellationToken);
        }

        public async Task Handle(TransactionDeletedEvent notification, CancellationToken cancellationToken)
        {

            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            await _categoryFilledLevel.UpdateSumAfterTransactionDeleted(notification.TransactionId, account.Id, cancellationToken);
        }
    }
}
