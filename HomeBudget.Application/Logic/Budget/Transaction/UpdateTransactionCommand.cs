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

namespace HomeBudget.Application.Logic.Budget.Transaction
{
    public static class UpdateTransactionCommand
    {
        public class Request : IRequest<Result>
        {
            public required int Id { get; set; }

            public required string Name { get; set; }

            public required int CategoryId { get; set; }

            public required int PreviousCategoryId { get; set; }

            public required DateTimeOffset Date { get; set; }

            public DateTimeOffset PreviousDate { get; set; }

            public required decimal Amount { get; set; }
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



                var categoryExist = await _applicationDbContext.Transactions.AnyAsync(y =>  y.Name == request.Name && y.CategoryId == request.CategoryId && y.AccountId == account.Id && y.Date == request.Date && y.Amount == request.Amount && !y.IsDeleted);

                if (categoryExist)
                {
                    throw new ErrorException("TranscationDidNotChange");
                }

                Domain.Entities.Budget.Transaction? model = null;

                model = await _applicationDbContext.Transactions.FirstOrDefaultAsync(c => c.Id == request.Id && c.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }

                model.Name = request.Name;
                model.Date = request.Date;
                model.CategoryId = request.CategoryId;
                model.Amount = request.Amount;
            
                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                await _mediator.Publish(new TransactionUpdatedEvent
                {
                    TransactionId = model.Id,
                    CategoryId = model.CategoryId,
                    AccountId = model.AccountId,
                    Amount = model.Amount,
                    Name = model.Name,
                    Date = model.Date,
                    PreviousCategoryId = request.PreviousCategoryId,
                    PreviousDate = request.PreviousDate
                }, cancellationToken);

                return new Result();
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.Name).NotEmpty();
                    RuleFor(x => x.Name).MaximumLength(50);
                    RuleFor(x => x.Amount).PrecisionScale(8, 2, true);
                }
            }
        }
    }
}
