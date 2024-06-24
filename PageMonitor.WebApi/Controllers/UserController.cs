using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PageMonitor.Application.Logic.User;

namespace PageMonitor.WebApi.Controllers
{

    // zmienamy routing w tej chwili dzialat tak, ze mamy adres api
    // a nastepnie adres kontrollera
    //[Route("api/[controller]")]
    // natomiast my jeszcze chcemy adres akcji dlatego tez
    // wiec teraz nie ma prefiksu api
    // nestepnie jest nazwa kontrolera i akcji ktora wywolujemy
    [Route("[controller]/[action]")]
    [ApiController]
    //dzieiczy po BaseControllerze
    public class UserController : BaseController
    {
        public UserController(ILogger<UserController> logger, IMediator mediator) : base(logger, mediator)
        {
        }

        // dodajemy akcje, ktora wywola naszego commanda
        // akcja Create typu POST tworzącą nowe konto w aplikacji
        // dobrze, zeby nazwa odpowiadala nazwa commanda
        // w parametrze jest request, czyli nasz request z commanda
        // zawartosc dane do tej klasy sa odczytywane z body requesta
        // jest to requesta typu post
        [HttpPost]
        public async Task<ActionResult> CreateUserWithAccount([FromBody] CreateUserWithAccountCommand.Request model)
        {
            // to co robimy to bierzemy mediatora i wysylamy obiekt o nazwie model (nasz request)
            // mediator zajmie sie wyslaniem tego i przekazaniem do handlera
            // ktory znajduje sie w klasie CreateUserWithAccountCommand
            // w projekcie Application
            var createAccountResult = await _mediator.Send(model);
            return Ok(createAccountResult);
        }
    }
}
