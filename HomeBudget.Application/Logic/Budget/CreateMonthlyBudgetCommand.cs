using FluentValidation;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities.Budget.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Budget
{
    public static class CreateMonthlyBudgetCommand
    {
        public class Request : IRequest<Result>
        {
            public required int YearBudgetId { get; set; }

            public required Month Month { get; set; }

            public required decimal TotalAmount { get; set; }
        }

        public class Result
        {
            public int MonthlyBudgetId {  get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = _currentAccountProvider.GetAuthenticatedAccount();


                var monthlyBudget = new MonthlyBudget()
                {
                    YearBudgetId = request.YearBudgetId,
                    Month = request.Month,
                    TotalAmount = request.TotalAmount
                };

                _applicationDbContext.MonthlyBudgets.Add(monthlyBudget);

                await _applicationDbContext.SaveChangesAsync();


                return new Result()
                {
                    MonthlyBudgetId = monthlyBudget.Id
                };
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.YearBudgetId).NotEmpty();
                    RuleFor(x => x.Month).IsInEnum();
                    RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0);
                }
            }
        }
    }
}
