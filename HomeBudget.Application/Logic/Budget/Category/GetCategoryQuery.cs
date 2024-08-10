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
using static HomeBudget.Application.Logic.Budget.Category.GetCommandQuery.Result;

namespace HomeBudget.Application.Logic.Budget.Category
{
    public static class GetCommandQuery
    {
        public class Request :IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class Result 
        {
                public required string Name { get; set; }

                public required CategoryType CategoryType { get; set; }

                public required bool IsDraft { get; set; }
        }


        public class Handler : BaseQueryHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var model = await _applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id && c.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }

                return new Result
                {
                        Name = model.Name,
                        CategoryType = model.CategoryType,
                        IsDraft = model.IsDraft,
                };
            }
        }
    }
}
