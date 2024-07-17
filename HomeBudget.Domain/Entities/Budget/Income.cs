using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class Income : DomainEntity
    {
        public int MonthlyBudgetId { get; set; }

        public int BudgetCategoryId { get; set; }

        public decimal Amount { get; set; }

        public bool IsPlanned { get; set; }

        public DateTimeOffset Date { get; set; }

        public string? Description { get; set; }

        public MonthlyBudget MonthlyBudget { get; set; } = default!;

        public IncomeCategory IncomeCategory { get; set; } = default!;

    }
}
