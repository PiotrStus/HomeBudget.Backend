using HomeBudget.Domain.Entities.Budget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Persistance.Configurations
{
    public class YearBudgetConfiguration : IEntityTypeConfiguration<YearBudget>
    {
        public void Configure(EntityTypeBuilder<YearBudget> builder)
        {
            builder.HasOne(y => y.Account)
                   .WithMany(a => a.YearsBudgets)
                   .HasForeignKey(k => k.AccountId);

            builder.HasMany(y => y.MonthlyBudgets)
                   .WithOne(m => m.YearBudget)
                   .HasForeignKey(k => k.YearBudgetId);

            builder.HasMany(y => y.Expenses)
                   .WithOne(m => m.YearBudget)
                   .HasForeignKey(k => k.YearBudgetId);

            builder.HasMany(y => y.Goals)
                   .WithOne(m => m.YearBudget)
                   .HasForeignKey(k => k.YearBudgetId);
        }
    }
}
