using HomeBudget.Application.Logic.Budget;
using HomeBudget.Application.Logic.Budget.YearBudget;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class BudgetController : BaseController
    {
        public BudgetController(ILogger<MonthlyBudgetController> logger, IMediator mediator) : base(logger, mediator)
        {
        }

        [HttpPost]
        public async Task<ActionResult> CreateYearBudget([FromBody] CreateYearBudgetCommand.Request model)
        {
            var createYearResult = await _mediator.Send(model);
            return Ok(createYearResult);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteYearBudget([FromBody] DeleteYearBudgetCommand.Request model)
        {
            var DeleteYearBudgetResult = await _mediator.Send(model);
            return Ok(DeleteYearBudgetResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetBudgets([FromQuery] GetBudgetsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
