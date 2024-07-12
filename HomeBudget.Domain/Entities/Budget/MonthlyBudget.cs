using HomeBudget.Domain.Common;
using HomeBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget.Budget
{
    public class MonthlyBudget : DomainEntity
    {
        public int YearBudgetId { get; set; }

        public Months Month { get; set; }

        public decimal TotalAmount { get; set; }

        public YearBudget YearBudget { get; set; }

        public ICollection<Expense> PlannedExpenses { get; set; } = new List<Expense>();

        public ICollection<Expense> ActualExpenses { get; set; } = new List<Expense>();

        public ICollection<Income> PlannedIncome { get; set; } = new List<Income>();

        public ICollection<Income> ActualIncome { get; set; } = new List<Income>();

        public ICollection<DraftExpense> PlannedDraftExpenses { get; set; } = new List<DraftExpense>();
    }
}
