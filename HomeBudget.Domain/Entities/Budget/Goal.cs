using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class Goal : DomainEntity
    {
        public required string Name { get; set; }

        public int YearBudgetId { get; set; }

        public required decimal StartingAmount { get; set; }

        public required decimal TargetAmount { get; set; }

        public required DateTimeOffset StartTime { get; set; }

        public required DateTimeOffset EndTime { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; } = default!;

        public string? Description { get; set; }

        public YearBudget YearBudget { get; set; } = default!;

    }
}
