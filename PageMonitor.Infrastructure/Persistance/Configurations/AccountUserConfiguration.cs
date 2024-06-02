using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PageMonitor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Infrastructure.Persistance.Configurations
{
    // IEntityTypeConfiguration -> interfejs z EF, ktory pozwala na ustawienie konfiguracji dla dengo typu encji
    public class AccountUserConfiguration : IEntityTypeConfiguration<AccountUser>
    {
        public void Configure(EntityTypeBuilder<AccountUser> builder)
        {
            builder.HasOne(p => p.Account)
                .WithMany(a => a.AccountUsers)
                .HasForeignKey(k => k.AccountId);

            builder.HasOne(p => p.User)
                .WithMany(a => a.AccountUsers)
                .HasForeignKey(k => k.UserId);
        }
    }
}

