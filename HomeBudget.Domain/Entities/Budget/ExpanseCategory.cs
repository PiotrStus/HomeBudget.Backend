using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class ExpenseCategory : DomainEntity
    {
        public required string Name { get; set; }

        public required string Description { get; set; }

        public ICollection<ExpenseSubcategory> Subcategories { get; set; } = new List<ExpenseSubcategory>();

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    }
}
