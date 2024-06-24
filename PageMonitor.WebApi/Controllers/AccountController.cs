using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PageMonitor.Application.Logic.Account;
using PageMonitor.Application.Logic.User;
using PageMonitor.Infrastructure.Auth;
using PageMonitor.WebApi.Application.Auth;
using PageMonitor.WebApi.Application.Response;

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
    public class AccountController : BaseController
    {

        public AccountController(ILogger<AccountController> logger,
            IMediator mediator) : base(logger, mediator)
        {
        }

        // potrzebujemy w zasadzie tylko jednej metody, czyli pobierania naszego
        // konta
        [HttpGet]
        public async Task<ActionResult> GetCurrentAccount()
        {
            var data = await _mediator.Send(new CurrentAccountQuery.Request() { });
            return Ok(data);
        }
    }
}
