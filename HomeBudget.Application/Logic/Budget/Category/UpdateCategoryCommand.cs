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
    public static class UpdateCategoryCommand
    {
        public class Request : IRequest<Result>
        {
            public required int Id { get; set; }

            public required string Name { get; set; }

            public required CategoryType CategoryType { get; set; }

            public required bool IsDraft { get; set; }
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

                var categoryExist = await _applicationDbContext.Categories.AnyAsync(y =>  y.Name == request.Name && y.CategoryType == request.CategoryType && y.AccountId == account.Id && y.IsDraft == request.IsDraft && !y.IsDeleted);

                if (categoryExist)
                {
                    throw new ErrorException("CategoryDidNotChange");
                }

                Domain.Entities.Budget.Category? model = null;

                model = await _applicationDbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.Id && c.AccountId == account.Id);

                if (model == null)
                {
                    throw new UnauthorizedException();
                }

                model.Name = request.Name;
                model.CategoryType = request.CategoryType;
                model.IsDraft = request.IsDraft;
            
                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result();
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
