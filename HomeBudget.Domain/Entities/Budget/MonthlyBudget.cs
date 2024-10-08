﻿using HomeBudget.Domain.Common;
using HomeBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class MonthlyBudget : DomainEntity
    {
        public int YearBudgetId { get; set; }

        public required Month Month { get; set; }

        public required decimal TotalAmount { get; set; }

        public YearBudget YearBudget { get; set; } = default!;

        public ICollection<MonthlyBudgetCategory> MonthlyBudgetCategories { get; set; } = new List<MonthlyBudgetCategory>();
    }
}
