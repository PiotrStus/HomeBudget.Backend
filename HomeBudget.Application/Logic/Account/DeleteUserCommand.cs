using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Budget.Account
{
    public static class DeleteUserCommand
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

                if (account == null)
                {
                    throw new UnauthorizedException();
                }

                var model = await _applicationDbContext.AccountUsers.FirstOrDefaultAsync(au => au.UserId == request.Id && au.AccountId == account.Id && !au.IsAdmin);

                if (model == null)
                {
                    throw new ErrorException("CanNotDeleteAdmin");
                }

                _applicationDbContext.AccountUsers.Remove(model);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
