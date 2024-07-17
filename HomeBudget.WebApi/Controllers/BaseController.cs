using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{
    // jest to klasa abstrakcyjna, która dziediczy po ControllerBase
    // dzieki czemu nasze kontrolery nie będą już musiały po tej klasie dziediczyć
    public abstract class BaseController : ControllerBase
    {
        // w kazdym kontrolerze bedziemy miec loger
        protected readonly ILogger _logger;
        // i mediatora, dlatego ze za pomoca mediatora bedziemy wysylac nasze
        // query i commandy
        // mediatora mozna zamienic na interfejs ISender to jest interfejs z mediatora
        // ktory jest ograniczony do metody send
        // mediator zawiera jeszcze metode publish, ktora dodatkowo ma wysylac
        // rownolegle do roznych odbiorcow wiadomosci
        // tutaj z przyzwyczajenie uzywamy mediatora
        protected readonly IMediator _mediator;

        public BaseController(ILogger logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
    }
}
