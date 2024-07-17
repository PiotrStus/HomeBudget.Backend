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

        public Month Month { get; set; }

        public decimal TotalAmount { get; set; }

        public YearBudget YearBudget { get; set; } = default!;


        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();


        public ICollection<Income> Income { get; set; } = new List<Income>();
    }
}
