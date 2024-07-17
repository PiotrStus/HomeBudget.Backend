using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Entities.Budget.Budget;
using Microsoft.EntityFrameworkCore;
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

        DbSet<Account> Accounts { get; set; }

        DbSet<AccountUser> AccountUsers { get; set; }

        DbSet<DraftExpense> DraftExpenses { get; set; }

        DbSet<Expense> Expenses { get; set; }

        DbSet<ExpenseCategory> ExpenseCategories { get; set; }

        DbSet<ExpenseSubcategory> ExpenseSubcategories { get; set; }

        DbSet<Goal> Goals { get; set; }

        DbSet<Income> Incomes { get; set; }

        DbSet<IncomeCategory> IncomeCategories { get; set; }

        DbSet<MonthlyBudget> MonthlyBudgets { get; set; }

        DbSet<YearBudget> YearBudgets { get; set; }


        // sluzy do zapisu danych do BD, jej implementacje zawiera juz klasa DbContext z Entity Frameworka
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
