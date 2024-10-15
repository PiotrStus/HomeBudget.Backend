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
using static HomeBudget.Application.Logic.User.CreateUserCommand.Request;
using HomeBudget.Domain.Enums;

namespace HomeBudget.Application.Logic.User
{
    public static class ConfirmUserCommand
    {
        public class Request : IRequest<Result>
        {
            public required string UserGuid { get; set; }

        }

        public class Result
        {
            public required int UserId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext) : base(currentAccountProvider, applicationDbContext)
            {

            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var guidCheck = Guid.TryParse(request.UserGuid, out var guidValue);

                if (!guidCheck)
                {
                    throw new ErrorException("WrongGuidFormat");
                }

                var userGuid = await _applicationDbContext.UserConfirmGuids
                    .Include(u => u.User)
                    .FirstOrDefaultAsync(u => u.UserGuid == guidValue && u.GuidType == UserGuidType.ConfirmAccount);

                if (userGuid != null)
                {
                    userGuid.User.IsActivated = true;

                    var userId = userGuid.User.Id;

                    _applicationDbContext.UserConfirmGuids.Remove(userGuid);

                    await _applicationDbContext.SaveChangesAsync(cancellationToken);

                    return new Result()
                        {
                            UserId = userId
                        };
                    
                }

                throw new ErrorException("ActivationNotCompletedOrAlreadyActivated");
            }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.UserGuid).NotEmpty();
            }
        }
    }
}