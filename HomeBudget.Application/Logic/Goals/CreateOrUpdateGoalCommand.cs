using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Logic.Abstractions;
using HomeBudget.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Goals
{
    public static class CreateOrUpdateGoalCommand
    {
        public class Request : IRequest<Result>
        {
            public int? Id { get; set; }
            public required string Name { get; set; }
            public required decimal StartingAmount { get; set; }
            public required decimal TargetAmount { get; set; }
            public required DateTimeOffset StartTime { get; set; }
            public required DateTimeOffset EndTime { get; set; }
            public string Description { get; set; }
        }

        public class Result
        {
            public required int GoaldId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {

                var account = await _currentAccountProvider.GetAuthenticatedAccount();


                Domain.Entities.Budget.Goal? model = null;

                if (request.Id.HasValue)
                {
                    model = await _applicationDbContext.Goals.FirstOrDefaultAsync(g => g.Id == request.Id && g.AccountId == account.Id);
                }
                else
                {
                    model = new Domain.Entities.Budget.Goal()
                    {
                        Name = "",
                        StartingAmount = 0,
                        TargetAmount = 0,
                        StartTime = default,
                        EndTime = default,
                    };

                    _applicationDbContext.Goals.Add(model);
                }

                if (model == null)
                {
                    throw new UnauthorizedAccessException();
                }

                model.Name = request.Name;
                model.StartingAmount = request.StartingAmount;
                model.TargetAmount = request.TargetAmount;
                model.StartTime = request.StartTime;
                model.EndTime = request.EndTime;
                model.Description = request.Description;


                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                return new Result()
                {
                    GoaldId = model.Id,
                };
            }
        }





        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Name).NotEmpty();
                RuleFor(x => x.Name).MaximumLength(50);
                RuleFor(x => x.StartingAmount).NotEmpty();
                RuleFor(x => x.TargetAmount).NotEmpty();
                RuleFor(x => x.StartTime).NotEmpty();
                RuleFor(x => x.EndTime).NotEmpty();

                RuleFor(x => x.Description).MaximumLength(255);

                RuleFor(x => x.StartTime).LessThanOrEqualTo(DateTimeOffset.Now);

                RuleFor(x => x.EndTime).GreaterThanOrEqualTo(x => x.StartTime);

        }
        }

    }
}
