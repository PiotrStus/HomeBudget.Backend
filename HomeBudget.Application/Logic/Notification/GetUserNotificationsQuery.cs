using EFCoreSecondLevelCacheInterceptor;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Domain.Enums;

namespace HomeBudget.Application.Logic.Notification
{
    public static class GetUserNotificationsQuery
    {
        public class Request : IRequest<Result>
        {
        }

        public class Result
        {
            public required List<UserNotification> Notifications { get; set; } = new List<UserNotification>();

            public class UserNotification()
            {
                public required int Id { get; set; }
                public required DateTimeOffset Date { get; set; }

                public required string Content { get; set; }

                public required NotificationType NotificationType { get; set; }

                public required string CategoryName { get; set; }
            }
        }

        public class Handler : BaseQueryHandler, IRequestHandler<Request, Result>
        {
            private readonly IAuthenticationDataProvider _authenticationDataProvider;

            public Handler(ICurrentAccountProvider currentAccountProvider,
                IApplicationDbContext applicationDbContext,
                IAuthenticationDataProvider authenticationDataProvider
                ) : base(currentAccountProvider, applicationDbContext)
            {
                _authenticationDataProvider = authenticationDataProvider;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var userId = _authenticationDataProvider.GetUserId();

                if (userId.HasValue)
                {

                    var notifications = await _applicationDbContext.Notifications
                                                                   .Where(n => n.UserId == userId && !n.IsRead)
                                                                   .Select(n => new Result.UserNotification()
                                                                   {
                                                                       Id = n.Id,
                                                                       Date = n.Date,
                                                                       Content = n.Content,
                                                                       NotificationType = n.NotificationType,
                                                                       CategoryName = n.CategoryName,
                                                                   })
                                                                   .Cacheable()
                                                                   .ToListAsync();

                    if (notifications.Any())
                    {
                        return new Result()
                        {
                            Notifications = notifications
                        };
                    }
                    else
                    {
                        return new Result()
                        {
                            Notifications = []
                        };
                    }
                }
                throw new UnauthorizedException();
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
            }
        }

    }
}
