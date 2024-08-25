using FluentValidation;
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

namespace HomeBudget.Application.Logic.Budget.Category
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
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var plannedCategoryNotChanged = await _applicationDbContext.MonthlyBudgetCategories.AnyAsync(m => m.Amount == request.Amount && m.MonthlyBudget.YearBudget.AccountId == account.Id);

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
