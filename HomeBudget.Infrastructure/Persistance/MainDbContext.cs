using Microsoft.EntityFrameworkCore;
using PageMonitor.Application.Interfaces;
using PageMonitor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Infrastructure.Persistance
{
    public class MainDbContext : DbContext, IApplicationDbContext
    {
        // jak parametr konfiguracja db contextu, zeby podac connection stringa itp
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        { 
        }
        public DbSet<User> Users { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountUser> AccountUsers { get; set; }

        // mechanizm, ktory automatycznie zaimportuje konfiguracje z plikow z folderu Configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //typeof(MainDbContext).Assembly jako parametr z ktorego assembly chcemy wziac konfiguracje,
            //czyli w tym przypadku infrastructure
            // dzieki temu wszystkie klasy ktore dziedicza po IEntityTypeConfiguration beda uzyte do zastosowania konfiguracji
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainDbContext).Assembly);
        }

        // pozwala ustawic globalne ustawienia dla propertisow, dzieki czemu mozna zrobic to w jednym miejscu i bedzie obowiazywac we wszystkich encjach domenowych
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {

            // decimal w kodzie c# to bedzie precyzja 18 i 4 globalnie!!!
            configurationBuilder.Properties<decimal>().HavePrecision(18, 4);

            base.ConfigureConventions(configurationBuilder);
        }
    }
}
