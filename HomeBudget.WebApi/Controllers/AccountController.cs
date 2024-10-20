﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HomeBudget.Application.Logic.Account;
using HomeBudget.Application.Logic.User;
using HomeBudget.Infrastructure.Auth;
using HomeBudget.WebApi.Application.Auth;
using HomeBudget.WebApi.Application.Response;
using HomeBudget.Application.Logic.Budget.Account;
using HomeBudget.Application.Logic.Budget.Category;

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

        [HttpPost]
        public async Task<ActionResult> SwitchAccount([FromBody] SwitchAccountQuery.Request model)
        {
            var data = await _mediator.Send(model);
            if (data != null)
            {
                SetAccountIdCookie(data.Id);
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> AssignUser([FromBody] AssignUserCommand.Request model)
        {
            var data = await _mediator.Send(model);

            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteUserCommand.Request model)
        {
            var DeleteUserResult = await _mediator.Send(model);
            return Ok(DeleteUserResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetCurrentAccount()
        {
            var data = await _mediator.Send(new CurrentAccountQuery.Request() { });
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            var data = await _mediator.Send(new GetUsersQuery.Request() { });
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
