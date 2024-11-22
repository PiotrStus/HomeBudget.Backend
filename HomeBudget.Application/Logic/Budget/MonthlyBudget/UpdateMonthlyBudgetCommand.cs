using FluentValidation;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Enums;
using HomeBudget.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Budget.MonthlyBudget
{
    public static class UpdateMonthlyBudgetCommand
    {
        public class Request : IRequest<Result>
        {
            public required int Id { get; set; }

            public required int YearBudgetId { get; set; }

            public required Month Month { get; set; }

            public decimal? TotalAmount { get; set; }
        }

        public class Result
        {
            public int Id { get; set; }
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

                var monthlyBudgetNotChanged = await _applicationDbContext.MonthlyBudgets.AnyAsync(y => y.Id != request.Id && y.YearBudget.AccountId == account.Id && y.YearBudgetId == request.YearBudgetId && y.Month == request.Month);

                if (monthlyBudgetNotChanged)
                {
                    throw new ErrorException("MonthlyBudgetAlreadyExists");
                }

                Domain.Entities.Budget.MonthlyBudget? model = null;

                model = await _applicationDbContext.MonthlyBudgets.FirstOrDefaultAsync(m => m.Id == request.Id && m.YearBudget.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }

                // najpierw trzeba usunac ten co jest i wszystkie jego kategorie
                // a nastepnie dodac nowy i utworzyc dla niego kategorie

                model.YearBudgetId = request.YearBudgetId;
                model.Month = request.Month;
                model.TotalAmount = request.TotalAmount ?? 0m;

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                {
                    Id = model.Id
                };
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.YearBudgetId).NotEmpty();
                    RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0m);
                    RuleFor(x => x.Month).IsInEnum();
                }
            }
        }
    }
}
