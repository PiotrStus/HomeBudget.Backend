using HomeBudget.Application.Logic.Goals;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class BudgetController : BaseController
    {
        public BudgetController(ILogger logger, IMediator mediator) : base(logger, mediator)
        {
        }

        [HttpPost]

        public async Task<ActionResult> CreateOrUpdateGoal([FromBody] CreateOrUpdateGoalCommand.Request model )
        {
            var createOrUpdateResult = await _mediator.Send(model);
            return Ok(createOrUpdateResult);
        }

    }
}
