﻿using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Budget.MonthlyBudget
{
    public static class DeleteMonthlyBudgetCommand
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

                var model = await _applicationDbContext.MonthlyBudgets.FirstOrDefaultAsync(y => y.Id == request.Id && y.YearBudget.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }
                _applicationDbContext.MonthlyBudgets.Remove(model);


                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result();
            }
        }
    }
}
