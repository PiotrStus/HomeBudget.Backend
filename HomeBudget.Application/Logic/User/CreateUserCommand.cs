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
using Microsoft.Extensions.Configuration;
using HomeBudget.Application.Services;

namespace HomeBudget.Application.Logic.User
{
    public static class CreateUserCommand
    {
        public class Request : IRequest<Result>
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        public class Result
        {
            public required int UserId { get; set; }
        }

        public class Handler : BaseCommandHandler, IRequestHandler<Request, Result>
        {
            private readonly IPasswordManager _passwordManager;
            private readonly ILinkProvider _linkProvider;
            private readonly EmailManager _emailManager;

            public Handler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext, IPasswordManager passwordManager, ILinkProvider linkProvider, EmailManager emailManager) : base(currentAccountProvider, applicationDbContext)
            {
                _passwordManager = passwordManager;
                _linkProvider = linkProvider;
                _emailManager = emailManager;
            }

            public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
            {
                var userExists = await _applicationDbContext.Users.AnyAsync(u => u.Email == request.Email && u.IsActivated);
                if (userExists)
                {
                    throw new ErrorException("AccountWithThisEmailAlreadyExists");
                }

                var user = await _applicationDbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsActivated);

                var utcNow = DateTime.UtcNow;

                UserConfirmGuid? userGuid = null;

                if (user == null)
                {
                    user = new Domain.Entities.User()
                    {
                        RegisterDate = utcNow,
                        Email = request.Email,
                        IsActivated = false,
                        HashedPassword = "",
                    };

                    user.HashedPassword = _passwordManager.HashPassword(request.Password);

                    userGuid = new UserConfirmGuid()
                    {
                        User = user,
                        UserGuid = Guid.NewGuid(),
                        GuidType = UserGuidType.ConfirmAccount
                    };

                    _applicationDbContext.Users.Add(user);
                    _applicationDbContext.UserConfirmGuids.Add(userGuid);
                }
                else
                {
                    userGuid = await _applicationDbContext.UserConfirmGuids
                        .FirstOrDefaultAsync(g => g.UserId == user.Id && g.GuidType == UserGuidType.ConfirmAccount);

                    if (userGuid == null)
                    {
                        throw new ErrorException("NoConfirmationGuidFoundForUser");
                    }
                }

                await _applicationDbContext.SaveChangesAsync(cancellationToken);

                var confirmationLink = _linkProvider.GenerateConfirmationLink(userGuid.UserGuid);

                var model = new
                {
                    Username = user.Email,
                    ConfirmationLink = confirmationLink
                };

                await _emailManager.SendEmail("confirmAccount", user.Email, model);

                return new Result()
                { 
                    UserId = user.Id, 
                };
            }
        }

        // dodajemy to jako osobna klasa
        // on dziediczy po AbstractValidatorze to jest klasa z fluentvalidation
        // wszystkie klasy dziedziczace po AbstractValidator sa rejestrowane automatycznie w 
        // kontenerze DI dzieki metodzie AddValidatorsFromAssemblyContaining()
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                // Dodaj reguły walidacji
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Email).EmailAddress();
                RuleFor(x => x.Email).MaximumLength(100);

                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Password).MaximumLength(50);
            }
        }
    }
}