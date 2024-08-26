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

namespace HomeBudget.Application.Logic.EventHandlers.TransactionCreated
{
    public class TransactionCreatedEventHandler : BaseQueryHandler, INotificationHandler<TransactionCreatedEvent>
    {
        public TransactionCreatedEventHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
        {
        }

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            if (notification.Date != null)
            {
                var year = notification.Date.Value.Year;
                var monthNumber = notification.Date.Value.Month;

                Month? month = null;

                if (Enum.IsDefined(typeof(Month), monthNumber))
                {
                    month = (Month) monthNumber;
                }

                var monthlyBudget = await _applicationDbContext.MonthlyBudgets.FirstOrDefaultAsync(mb => mb.YearBudget.Year == year && mb.Month == month && notification.AccountId == account.Id);

                if (monthlyBudget == null)
                {
                    throw new ErrorException("MonthlyBudgetNotFound");
                }

                var category = await _applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Id == notification.CategoryId && c.AccountId == notification.AccountId);

                if (category == null)
                {
                    throw new ErrorException("MonthlyBudgetNotFound");
                }


                var monthlyBudgetCategory = await _applicationDbContext.MonthlyBudgetCategories
                                        .Include(mc => mc.MonthlyBudgetCategoryTracking)
                                        .FirstOrDefaultAsync(mc => mc.MonthlyBudgetId == monthlyBudget.Id && mc.CategoryId == category.Id);

                var tracking = monthlyBudgetCategory?.MonthlyBudgetCategoryTracking;

                if (tracking != null && monthlyBudgetCategory?.Amount > 0)
                {
                    
                    var level = (int) (notification.Amount / monthlyBudgetCategory.Amount * 100);

                    tracking.CategoriesFilledLevel = level;
                    await _applicationDbContext.SaveChangesAsync(cancellationToken);
                }
            }

            else
            {
                throw new ErrorException("CanNotCreateTransaction");
            }
        }
    }
}
