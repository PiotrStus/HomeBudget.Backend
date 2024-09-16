using MediatR;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using HomeBudget.Infrastructure.Auth;
using HomeBudget.WebApi.Application.Auth;
using HomeBudget.WebApi.Application.Response;
using HomeBudget.Application.Logic.Notification;

namespace HomeBudget.WebApi.Controllers
{


    [Route("[controller]/[action]")]
    [ApiController]
    public class NotificationController : BaseController
    {

        public NotificationController(ILogger<NotificationController> logger, IMediator mediator) : base(logger, mediator)
        {

        }


        [HttpPost]
        public async Task<ActionResult> ConfirmNotification([FromBody] ConfirmNotificationCommand.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetUserNotifications([FromQuery] GetUserNotificationsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
