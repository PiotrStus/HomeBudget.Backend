using FluentValidation;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities.Budget;
using HomeBudget.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Budget
{
    public static class CreateCategoryCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Name { get; set; }
            public required CategoryType CategoryType { get; set; }
        }

        public class Result
        {
            public int CategoryId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var account = await _currentAccountProvider.GetAuthenticatedAccount();

                var category = new Category()
                {
                    Name = request.Name,
                    CategoryType = request.CategoryType,
                    Account = account
                };

                _applicationDbContext.Categories.Add(category);


                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                {
                    CategoryId = category.Id
                };
            }

            public class Validator : AbstractValidator<Request>
            {
                public Validator()
                {
                    RuleFor(x => x.Name).NotEmpty();
                    RuleFor(x => x.Name).MaximumLength(50);
                    RuleFor(x => x.CategoryType).IsInEnum();
                }
            }
        }
    }
}
