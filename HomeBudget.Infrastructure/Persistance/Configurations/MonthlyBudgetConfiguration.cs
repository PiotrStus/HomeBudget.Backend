using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Entities.Budget.Budget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Persistance.Configurations
{
    public class MonthlyBudgetConfiguration : IEntityTypeConfiguration<MonthlyBudget>
    {
        public void Configure(EntityTypeBuilder<MonthlyBudget> builder)
        {
            builder.HasOne(y => y.YearBudget)
                   .WithMany(a => a.MonthlyBudgets)
                   .HasForeignKey(k => k.YearBudgetId);

            builder.HasMany(y => y.MonthlyBudgetCategories)
                   .WithOne(m => m.MonthlyBudget)
                   .HasForeignKey(k => k.MonthlyBudgetId);
        }
    }
}
