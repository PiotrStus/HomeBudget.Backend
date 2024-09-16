using FluentValidation;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Application.Logic.Events;
using HomeBudget.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Notification
{
    public static class ConfirmNotificationCommand
    {
        public class Request : IRequest<Result>
        {
            public required int Id { get; set; }

        }

        public class Result
        {

        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {

            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var userIds = await _applicationDbContext.AccountUsers
                    .Where(au => au.AccountId == account.Id)
                    .Select(au => au.UserId)
                    .ToListAsync(cancellationToken);

                var model = await _applicationDbContext.Notifications
                    .Where(n => n.Id == request.Id && userIds.Contains(n.UserId))
                    .FirstOrDefaultAsync(cancellationToken);


                if (model == null)
                {
                    throw new UnauthorizedException();
                }

                model.IsRead = true;

                await _applicationDbContext.SaveChangesAsync(cancellationToken);


                return new Result();
            }
        }
    }
}
