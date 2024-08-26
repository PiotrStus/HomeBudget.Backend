using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class MonthlyBudgetCategory : DomainEntity
    {
        public MonthlyBudgetCategory()
        {
            MonthlyBudgetCategoryTracking = new MonthlyBudgetCategoryTracking();

        }

        public int MonthlyBudgetId { get; set; }

        public MonthlyBudget MonthlyBudget { get; set; } = default!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = default!;

        public required decimal Amount { get; set; } = default!;

        public MonthlyBudgetCategoryTracking MonthlyBudgetCategoryTracking { get; set; } = default!;

    }
}
