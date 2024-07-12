using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class Goals : DomainEntity
    {
        public required string Name { get; set; }

        public int YearBudgetId { get; set; }

        public decimal StartingAmount { get; set; }

        public decimal TargetAmount { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public string Description { get; set; } // optional

        public YearBudget YearBudget { get; set; }

    }
}
