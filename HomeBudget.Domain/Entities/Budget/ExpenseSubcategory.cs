using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities
{
    public class ExpenseSubcategory : DomainEntity
    {
        public required string Name { get; set; }

        public string? Description { get; set; }

        public int BudgetCategoryId { get; set; }

        public ExpenseCategory ExpenseCategory { get; set; } = default!;

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
