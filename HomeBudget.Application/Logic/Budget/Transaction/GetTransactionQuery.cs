using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HomeBudget.Application.Logic.Budget.Category.GetTransactionQuery.Result;

namespace HomeBudget.Application.Logic.Budget.Category
{
    public static class GetTransactionQuery
    {
        public class Request :IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Result 
        {
                public required string Name { get; set; }

                public required DateTimeOffset Date { get; set; }

                public required int CategoryId { get; set; }

                public required decimal Amount { get; set; }
        }


        public class Handler : BaseQueryHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var model = await _applicationDbContext.Transactions.FirstOrDefaultAsync(c => c.Id == request.Id && c.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }

                return new Result
                {
                        Name = model.Name,
                        Date = model.Date,
                        CategoryId = model.CategoryId,
                        Amount = model.Amount
                };
            }
        }
    }
}
