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
    public static class GetTransactionsQuery
    {

        public class Request() : IRequest<Result>
        {
            public int Page { get; set; }

            public int PageSize { get; set; }

            public DateTimeOffset? Date { get; set; }

            public int? CategoryId { get; set; }

            public decimal? AmountMin { get; set; }

            public decimal? AmountMax { get; set; }

            public bool? CountPages { get; set; }
        }

        public class Result()
        {
            public required List<Transaction> Transactions { get; set; } = new List<Transaction>();

            public int? TotalCount { get; set; }

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

                var query = _applicationDbContext.Transactions
                    .Where(t => t.AccountId == account.Id && !t.IsDeleted);

                if (request.Date != null)
                {
                    query = query.Where(t => t.Date == request.Date);
                }

                if (request.CategoryId != null)
                {
                    query = query.Where(t => t.Category.Id == request.CategoryId);
                }

                if (request.AmountMin != null)
                {
                    query = query.Where(t => t.Amount >= request.AmountMin);
                }

                if (request.AmountMax != null)
                {
                    query = query.Where(t => t.Amount <= request.AmountMax);
                }

                int? totalCount = null;
                if (request.Page == 1 || request.CountPages == true)
                {
                    totalCount = await query.CountAsync(cancellationToken);
                }

                var transactions = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
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
                    Transactions = transactions,
                    TotalCount = totalCount
                };
            }
        }
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.AmountMin).PrecisionScale(8,2,true);
                RuleFor(x => x.AmountMax).PrecisionScale(8,2,true);
            }
        }
    }
}
