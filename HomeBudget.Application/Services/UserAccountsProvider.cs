using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Services
{
    public class UserAccountsProvider : IUserAccountsProvider
    {
        private readonly IAuthenticationDataProvider _authenticationDataProvider;
        private readonly IApplicationDbContext _applicationDbContext;

        public UserAccountsProvider(IAuthenticationDataProvider authenticationDataProvider, IApplicationDbContext applicationDbContext)
        {
            _authenticationDataProvider = authenticationDataProvider;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<Account>> GetUserAccounts()
        {
            var userId = _authenticationDataProvider.GetUserId();

            if (userId == null)
            {
                throw new UnauthorizedException();
            }

            return await _applicationDbContext.AccountUsers
                .Where(au => au.UserId == userId.Value)
                .Select(au => au.Account)
                .ToListAsync();
        }
    }
}