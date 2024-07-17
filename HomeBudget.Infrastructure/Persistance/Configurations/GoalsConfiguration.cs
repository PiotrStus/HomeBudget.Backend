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
    public class GoalsConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.HasOne(g => g.YearBudget)
                   .WithMany(y => y.Goals)
                   .HasForeignKey(k => k.YearBudgetId);

            builder.HasOne(g => g.Account)
                   .WithMany(a => a.Goals)
                   .HasForeignKey(k => k.AccountId);
        }
    }
}
