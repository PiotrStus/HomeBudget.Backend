using PageMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }

        DbSet<Account> Accounts { get; set; }

        DbSet<AccountUser> AccountUsers { get; set; }

        // sluzy do zapisu danych do BD, jej implementacje zawiera juz klasa DbContext z Entity Frameworka
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
