using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class MonthlyBudgetCategoryTracking : DomainEntity
    {
        public int CategoriesFilledLevel { get; set; }

        public MonthlyBudgetCategory MonthlyBudgetCategory { get; set; } = default!;
    }
}
