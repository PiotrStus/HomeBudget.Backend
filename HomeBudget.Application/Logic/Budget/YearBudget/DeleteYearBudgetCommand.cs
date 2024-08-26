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

namespace HomeBudget.Application.Logic.Budget.YearBudget
{
    public static class DeleteYearBudgetCommand
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

                var model = await _applicationDbContext.YearBudgets.FirstOrDefaultAsync(y => y.Id == request.Id && y.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }


                _applicationDbContext.YearBudgets.Remove(model);

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
