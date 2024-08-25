using HomeBudget.Application.Logic.Budget;
using HomeBudget.Application.Logic.Budget.Category;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class MonthlyBudgetController : BaseController
    {
        public MonthlyBudgetController(ILogger<MonthlyBudgetController> logger, IMediator mediator) : base(logger, mediator)
        {
        }
        [HttpPost]
        public async Task<ActionResult> CreateMonthlyBudget([FromBody] CreateMonthlyBudgetCommand.Request model)
        {
            var createMonthlyBudgetResult = await _mediator.Send(model);
            return Ok(createMonthlyBudgetResult);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateMonthlyBudget([FromBody] UpdateMonthlyBudgetCommand.Request model)
        {
            var UpdateMonthlyBudget = await _mediator.Send(model);
            return Ok(UpdateMonthlyBudget);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteMonthlyBudget([FromBody] DeleteMonthlyBudgetCommand.Request model)
        {
            var deleteMonthlyBudgetResult = await _mediator.Send(model);
            return Ok(deleteMonthlyBudgetResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetMonthlyBudget([FromQuery] GetMonthlyBudgetQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
