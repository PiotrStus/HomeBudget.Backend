using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HomeBudget.Application.Logic.Account;
using HomeBudget.Application.Logic.User;
using HomeBudget.Infrastructure.Auth;
using HomeBudget.WebApi.Application.Auth;
using HomeBudget.WebApi.Application.Response;

namespace HomeBudget.WebApi.Controllers
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
            IMediator mediator ) : base(logger, mediator)
        {
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountCommand.Request model)
        {
            var createAccountResult = await _mediator.Send(model);
            SetAccountIdCookie(createAccountResult.AccountId);
            return Ok(new { createAccountResult.AccountId });
        }

        [HttpGet]
        public async Task<ActionResult> GetCurrentAccount()
        {
            var data = await _mediator.Send(new CurrentAccountQuery.Request() { });
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> SwitchAccount([FromBody] SwitchAccountQuery.Request model)
        {
            var data = await _mediator.Send(model);
            if (data != null)
            {
                SetAccountIdCookie(data.VerifiedAccountId);
            }
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetUsersAccounts()
        {
            var data = await _mediator.Send(new GetUserAccountsQuery.Request() { });
            return Ok(data);
        }

        private void SetAccountIdCookie(int? accountId)
        {
            if (accountId != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(30),
                    SameSite = SameSiteMode.Lax
                };

                Response.Cookies.Append(CookieSettings.CookieAccountName, accountId.Value.ToString(), cookieOptions);
            }
        }
    }
}
