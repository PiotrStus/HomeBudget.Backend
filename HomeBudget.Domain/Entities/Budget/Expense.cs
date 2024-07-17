using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class Expense : DomainEntity
    {
        public int MonthlyBudgetId { get; set; }

        public int YearBudgetId { get; set; }

        public int BudgetCategoryId { get; set; }

        public int BudgetSubcategoryId { get; set; }

        public decimal Amount { get; set; }

        public bool IsPlanned {  get; set; }

        public DateTimeOffset Date { get; set; }

        public string? Description { get; set; }

        public MonthlyBudget MonthlyBudget { get; set; } = default!;

        public YearBudget YearBudget { get; set; } = default!;

        public ExpenseCategory ExpenseCategory { get; set; } = default!;

        public ExpenseSubcategory ExpenseSubcategory { get; set; } = default!;
    }
}
