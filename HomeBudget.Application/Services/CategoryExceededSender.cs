using EFCoreSecondLevelCacheInterceptor;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Services
{
    public class CategoryExceededSender
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICurrentAccountProvider _currentAccountProvider;

        public CategoryExceededSender(IApplicationDbContext applicationDbContext, ICurrentAccountProvider currentAccountProvider)
        {
            _applicationDbContext = applicationDbContext;
            _currentAccountProvider = currentAccountProvider;
        }
        
        public async Task SendNotification(int accountId, int categoryId, string content, CancellationToken cancellationToken)
        {
            var userIds = await _applicationDbContext.Users
                .Where(u => u.AccountUsers.Any(au => au.AccountId == accountId))
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);

            var account = await _currentAccountProvider.GetAuthenticatedAccount();

            if (account == null)
            {
                throw new UnauthorizedException();
            }

            if (userIds.Any())
            {
                var categoryName = await _applicationDbContext.Categories
                                        .Where(c => c.Id == categoryId)
                                        .Select(c => c.Name)
                                        .FirstOrDefaultAsync();
    

                foreach (var userId in userIds)
                {
                    var notification = new Notification()
                    {
                        Date = DateTimeOffset.Now,
                        UserId = userId,
                        AccountId = account.Id,
                        Content = content,
                        IsRead = false,
                        NotificationType = NotificationType.Warning,
                        CategoryName = categoryName ?? "UnknownCategory"
                    };
                
                    _applicationDbContext.Notifications.Add(notification);
                }

                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
