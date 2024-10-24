﻿using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }

        DbSet<UserConfirmGuid> UserConfirmGuids { get; set; }

        DbSet<Account> Accounts { get; set; }

        DbSet<AccountUser> AccountUsers { get; set; }

        DbSet<Transaction> Transactions { get; set; }

        DbSet<Category> Categories { get; set; }

        DbSet<MonthlyBudget> MonthlyBudgets { get; set; }

        DbSet<MonthlyBudgetCategory> MonthlyBudgetCategories { get; set; }

        DbSet<YearBudget> YearBudgets { get; set; }

        DbSet<Notification> Notifications { get; set; }


        // sluzy do zapisu danych do BD, jej implementacje zawiera juz klasa DbContext z Entity Frameworka
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
