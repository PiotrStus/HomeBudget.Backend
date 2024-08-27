using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class Transaction : DomainEntity
    {
        public required string Name { get; set; } = default!;

        public required DateTimeOffset Date { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; } = default!;

        public int CategoryId { get; set; }

        public Category? Category { get; set; } = default;

        public required decimal Amount {  get; set; }

    }
}
