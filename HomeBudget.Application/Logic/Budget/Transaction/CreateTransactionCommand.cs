using FluentValidation;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Domain.Entities.Budget;
using BudgetTransaction = HomeBudget.Domain.Entities.Budget.Transaction;
using HomeBudget.Application.Logic.Events;
using Microsoft.EntityFrameworkCore;


namespace HomeBudget.Application.Logic.Budget.Transaction
{
    public static class CreateTransactionCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Name { get; set; }

            public required int CategoryId { get; set; }

            public DateTimeOffset? Date { get; set; }

            public required decimal Amount { get; set; }

        }

        public class Result
        {
            public int TransactionId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IMediator _mediator;

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, IMediator mediator) : base(currentAccountProvider, applicationDbContext)
            {
                _mediator = mediator;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var utcNow = DateTime.UtcNow;

                var transaction = new BudgetTransaction()
                {
                    Name = request.Name,
                    Date = request.Date ?? utcNow,
                    CategoryId = request.CategoryId,
                    AccountId = account.Id,
                    Amount = request.Amount,
                };

                _applicationDbContext.Transactions.Add(transaction);


                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                await _mediator.Publish(new TransactionCreatedEvent 
                    { 
                        TransactionId = transaction.Id,
                        CategoryId = transaction.CategoryId,
                        AccountId = transaction.AccountId,
                        Amount = transaction.Amount,
                        Name = transaction.Name,
                        Date = transaction.Date,
                    }, cancellationToken);


                return new Result()
                {
                    TransactionId = transaction.Id
                };
            }
        }



        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Name).MaximumLength(100);
                RuleFor(x => x.CategoryId).NotEmpty();
                RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
                RuleFor(x => x.Amount).PrecisionScale(8, 2, true);
            }
        }

    }
}
