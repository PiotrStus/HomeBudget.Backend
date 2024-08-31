using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Application.Logic.Events;
using HomeBudget.Application.Services;
using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using MediatR;

namespace HomeBudget.Application.Logic.EventHandlers.TransactionCreated
{
    public class CalculateRealMonthlyBudgetHandler : BaseEventHandler, INotificationHandler<TransactionCreatedEvent>, INotificationHandler<TransactionDeletedEvent>
    {
        private readonly CategoryFilledLevel _categoryFilledLevel;


        public CalculateRealMonthlyBudgetHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, CategoryFilledLevel categoryFilledLevel) : base(currentAccountProvider, applicationDbContext)
        {
            _categoryFilledLevel = categoryFilledLevel;
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

            await _categoryFilledLevel.UpdateSumAfterTransactionChanged(transactionData, cancellationToken);
        }

        public async Task Handle(TransactionDeletedEvent notification, CancellationToken cancellationToken)
        {

            var account = await _currentAccountProvider.GetAuthenticatedAccount();


            await _categoryFilledLevel.UpdateSumAfterTransactionChanged(notification.TransactionId, account.Id, cancellationToken);
        }
    }
}
