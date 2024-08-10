using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget.Budget;
using HomeBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class Category : DomainEntity
    {
        public required string Name { get; set; }

        public required CategoryType CategoryType { get; set; }

        public required bool IsDraft { get; set; }

        public int AccountId { get; set; }


        public Account Account { get; set; } = default!;

        public bool IsDeleted { get; set; }

        //public bool MonthSpecific { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public ICollection<MonthlyBudgetCategory> MonthlyBudgetCategories { get; set; } = new List<MonthlyBudgetCategory>();

    }
}
