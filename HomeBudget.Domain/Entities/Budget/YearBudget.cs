using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class YearBudget : DomainEntity
    {
        public int Year { get; set; }

        public decimal TotalAmount { get; set; }

        public string Description { get; set; } // optional

        public ICollection<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();

        public ICollection<Expense> PlannedExpenses { get; set; } = new List<Expense>();

        public ICollection<Expense> ActualExpenses { get; set; } = new List<Expense>();

        public ICollection<Income> PlannedIncome { get; set; } = new List<Income>();

        public ICollection<Income> ActualIncome { get; set; } = new List<Income>();

        public ICollection<Goals> Goals { get; set; } = new List<Goals>();
    }
}
