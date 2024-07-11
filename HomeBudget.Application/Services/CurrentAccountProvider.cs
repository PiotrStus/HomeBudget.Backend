using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using PageMonitor.Application.Exceptions;
using PageMonitor.Application.Interfaces;
using PageMonitor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Application.Services
{
    public class CurrentAccountProvider : ICurrentAccountProvider
    {
        private readonly IAuthenticationDataProvider _authenticationDataProvider;
        private readonly IApplicationDbContext _applicationDbContext;

        // pobranie informacji o aktualnie zalogowanym koncie potrzebujemy dwóch rzeczy:
        // 1) dostępu do bazy danych i 2) pobranie ID aktualnie zalogowanego użytkownika
        // zrobimy to poprzez wstrzyknięcie odpowiedniej zależności przez konstruktor

        public CurrentAccountProvider(IAuthenticationDataProvider authenticationDataProvider, IApplicationDbContext applicationDbContext)
        {
            // dzieki temu mozna uzyc implementacji tych inteferjsow do wyciagniecia potzrebnych danych
            _authenticationDataProvider = authenticationDataProvider;
            _applicationDbContext = applicationDbContext;
        }

        // wyciaga Id konta na podstawie aktualnie zalogowanego użytkownika
        // to jest implementacja naszej metody z interfejsu ICurrentAccountProvider
        public async Task<int?> GetAccountId()
        {
            // pobranie Id aktualnie zalogowanego użytkownika
            var userId = _authenticationDataProvider.GetUserId();

            // pobranie z bazy danych id konta powiazanego z uzytjkownikiem

            if (userId != null)
                // pobieramy AccountUsers, w ktorych
                return await _applicationDbContext.AccountUsers
                    // userId jest tym naszym userId
                    .Where(au => au.UserId == userId.Value)
                    // sortujemy po Id, jesli jest kilka
                    .OrderBy(au => au.UserId)
                    // Select przekształca wyniki, wybierając tylko AccountId z każdego AccountUser.
                    // (int ?) oznacza rzutowanie na nullable int(int ?), co pozwala na zwrócenie null w przypadku braku wyników.
                    .Select(au => (int?)au.AccountId)
                    // metoda rozszerzajaca Cacheable, ktora wlacza cache dla tego zapytania
                    .Cacheable()
                    // wybieramy pierwsze id -> pierwsze konto
                    .FirstOrDefaultAsync(); // => pobranie top1 wiersza w SQL, nalezy je posortowac, bo jesli nie, to np. wykonujac te samo zapytanie, mozemy dostac inne wyniki

            return null;
        }

        // to jest implementacja naszej metody z interfejsu ICurrentAccountProvider
        // 
        public async Task<Account> GetAuthenticatedAccount()
        {
            // najpierw pobieramy sobie Account Id wywolujac metode GetAccountId
            var accountId = await GetAccountId();
            // jesli jest nullem, to uzywkonik np. nie jest zalogowany albo nie istnieje w bazie danych
            if (accountId == null)
            {
                throw new UnauthorizedException();
            }
            // pobieramy to konto z bazy danych
            var account = await _applicationDbContext.Accounts.Cacheable().FirstOrDefaultAsync(a => a.Id == accountId.Value);
            // np. w miedzyczasie ktos to konto usunal albo gdzies jest jakies zapomniane id konta, ktore juz nie istnieje w bazie
            if (account == null)
            {
                throw new ErrorException("AccountDoesNotExist");
            }
            return account;
        }
    }
}
