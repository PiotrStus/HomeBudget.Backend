using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Application.Logic.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HomeBudget.Application.Logic.Budget.Transaction
{
    public static class DeleteTransactionCommand
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
            private readonly IMediator _mediator;

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, IMediator mediator) : base(currentAccountProvider, applicationDbContext)
            {
                _mediator = mediator;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var model = await _applicationDbContext.Transactions.FirstOrDefaultAsync(c => c.Id == request.Id && c.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }

                model.IsDeleted = true;

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                await _mediator.Publish(new TransactionDeletedEvent
                {
                    TransactionId = model.Id,
                }, cancellationToken);

                return new Result();
            }
        }
    }
}
