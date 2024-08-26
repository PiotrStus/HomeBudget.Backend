using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class YearBudget : DomainEntity
    {
        public required int Year { get; set; }

        public string? Description { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; } = default!;

        public ICollection<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();

    }
}
