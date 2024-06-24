using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore.Update.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Application.Logic.Validators
{
    // IPipelineBehavior to jest interfejs z mediatora
    // on wpina się w dowolny request i dowolny response
    // rejestracje w kontenerze DI robimy za chwile
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        // lista walidatorow
        private readonly IList<IValidator<TRequest>> _validators;

        // w konstruktorze wstrzykujemy sobie liste walidatorow do danego requesta
        // ona bedzie tez automatycznie wstrzykiwana, poniewaz te walidatory
        // beda automatycznie zarejestrowane w konenetrze DI za pomoca metody, ktora
        // dodamy za chwile
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            // w tym propertiesie bedziemy miec liste walidatorow dla danego commanda
            // lub query
            _validators = validators.ToList();
        }


        // jak obsluzymy walidacje?

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // sprawdzamy czy ta lista walidatorow zawiera jakis walidator
            if (!_validators.Any())
            {
                // jesli nie to idziemy dalej
                return await next();
            }

            // jesli jest jakis walidator to
            // tworzymy validation context do ktorego przekazujemy obiekt request
            // ktorego bedziemy walidowac
            // to bedzie nasz np. z Command - klasa CreateUserWithAccountCommands.cs tam jest w srodku klasa Request

            var context = new ValidationContext<TRequest>(request);
            // ValidationContext klasa pomocnicza z nugeta fluen validation
            // nastetpnie lecimy po tych walidatorach, ktore mamy na liscie
            var errors = _validators
                // na kazdym z nich wykonujemy metode validate wlasnie z tym kontekstem
                // ktory zawiera nasz obiekt do walidacji
                .Select(v => v.Validate(context))
                // nastepnie wybieramy sobie bledy z klasy ValidationFailure
                // ona jest klasa z fluent validation
                .SelectMany(v => v.Errors)
                // takie bledy ktore nie sa nullem
                .Where(e => e != null)
                // grupujemy je po propertyname i kodzie bledu, dlatego ze moze wystapic
                // wiele takich samych bledow do tego samego property z tym samym kodem
                // a my chcemy miec je zgrupowane po to aby nie powielac wierszy
                .GroupBy(e => new { e.PropertyName, e.ErrorCode })
                .ToList();

            // sprawdzany czy ta lista zawiera jakis blad
            if (errors.Any())
            {
                // jesli tak to rzucamy wyjatek
                throw new Exceptions.ValidationException()
                {
                    // zwykle mapowanie bledow z klasy fluent validatora na nasza zwykla klase
                    // robimy to po to, poniewaz to jest nasza klasa i chcemy miec nad nia kontrole
                    // wiemy, ze nic sie nie zmieni po updatdzie, zewnetrzej lib
                    Errors = errors.Select(e => new Exceptions.ValidationException.FieldError()
                    {
                        Error = e.Key.ErrorCode,
                        FieldName = e.Key.PropertyName
                    }).ToList()
                };
            }
            //jesli nie ma bledow to przechodzimy dalej
            return await next();
        }
    }
}
