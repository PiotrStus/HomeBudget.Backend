using FluentValidation;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Budget.Transaction
{
    public static class GetRecentTransactionsQuery
    {

        public class Request() : IRequest<Result>
        {
            public required DateTimeOffset Date { get; set; }

            public required int Count { get; set; }
        }

        public class Result()
        {
            public required List<Transaction> Transactions { get; set; } = new List<Transaction>();

            public class Transaction()
            {
                public required int Id { get; set; }

                public required string Name { get; set; }

                public required DateTimeOffset Date { get; set; }

                public required int CategoryId { get; set; }

                public required string CategoryName { get; set; }

                public required decimal Amount { get; set; }

            }
        }

        public class Handler : BaseQueryHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();


                var monthFromRequest = (Month)request.Date.Month;


                var query = _applicationDbContext.Transactions
                    .Where(t => t.AccountId == account.Id && !t.IsDeleted && (Month)t.Date.Month == monthFromRequest && t.Date.Year == request.Date.Year)
                    .OrderByDescending(t => t.CreationDate)
                    .Take(request.Count);



                var transactions = await query
                    .Select(t => new Result.Transaction
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Date = t.Date,
                        CategoryName = t.Category.Name,
                        CategoryId = t.Category.Id,
                        Amount = t.Amount
                    })
                    .ToListAsync(cancellationToken);

                return new Result()
                {
                    Transactions = transactions
                };
            }
        }
    }
}
