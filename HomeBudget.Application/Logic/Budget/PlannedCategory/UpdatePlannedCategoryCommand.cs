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

namespace HomeBudget.Application.Logic.Budget.PlannedCategory
{
    public static class UpdatePlannedCategoryCommand
    {
        public class Request : IRequest<Result>
        {
            public required int Id { get; set; }

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

                var plannedCategoryNotChanged = await _applicationDbContext.MonthlyBudgetCategories.AnyAsync(m => m.Id == request.Id && m.Amount == request.Amount && m.MonthlyBudget.YearBudget.AccountId == account.Id);

                if (plannedCategoryNotChanged)
                {
                    throw new ErrorException("PlannedCategoryDidNotChange");
                }

                var plannedMonthlyBudget = await _applicationDbContext.MonthlyBudgetCategories.FirstOrDefaultAsync(m => m.Id == request.Id && m.MonthlyBudget.YearBudget.AccountId == account.Id);

                if (plannedMonthlyBudget == null)
                {
                    throw new UnauthorizedException();
                }

                plannedMonthlyBudget.Amount = request.Amount;

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                await _mediator.Publish(new PlannedCategoryAmountChangedEvent
                {
                    Id = request.Id,
                    CategoryId = plannedMonthlyBudget.CategoryId,
                    Amount = request.Amount,
                }, cancellationToken);

                return new Result();
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {

                }
            }
        }
    }
}
