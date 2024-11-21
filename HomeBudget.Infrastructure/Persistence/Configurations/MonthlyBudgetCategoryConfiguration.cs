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
    public class MonthlyBudgetCategoryConfiguration : IEntityTypeConfiguration<MonthlyBudgetCategory>
    {
        public void Configure(EntityTypeBuilder<MonthlyBudgetCategory> builder)
        {
            builder.HasOne(p => p.MonthlyBudget)
                   .WithMany(m => m.MonthlyBudgetCategories)
                   .HasForeignKey(k => k.MonthlyBudgetId);

            builder.HasOne(p => p.Category)
                   .WithMany(c => c.MonthlyBudgetCategories)
                   .HasForeignKey(k => k.CategoryId);

            // monthlyBudgetCategory ma jeden Tracking
            builder.HasOne(p => p.MonthlyBudgetCategoryTracking)
                    // tracking ma jeden monthlyBudgetCategory
                   .WithOne(t => t.MonthlyBudgetCategory)
                   // kolumna id w tracking jest kluczem obcym
                   // ten klucz odnosi sie do klucza glownego w monthlyBudgetCategory
                   .HasForeignKey<MonthlyBudgetCategoryTracking>(k => k.Id);
        }
    }
}
