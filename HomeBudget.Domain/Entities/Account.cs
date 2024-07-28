using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities
{
    public class Account : DomainEntity
    {
        public required string Name { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();

        public ICollection<YearBudget> YearsBudgets { get; set;} = new List<YearBudget>();

        public ICollection<Category> Categories { get; set; } = new List<Category>();  

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
