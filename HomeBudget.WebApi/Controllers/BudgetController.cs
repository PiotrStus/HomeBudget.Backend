using HomeBudget.Application.Logic.Budget;
using HomeBudget.Application.Logic.Budget.Category;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class BudgetController : BaseController
    {
        public BudgetController(ILogger<BudgetController> logger, IMediator mediator) : base(logger, mediator)
        {
        }


        [HttpPost]
        public async Task<ActionResult> CreateTransaction([FromBody] CreateTransactionCommand.Request model)
        {
            var createTransactionResult = await _mediator.Send(model);
            return Ok(createTransactionResult);
        }

        [HttpPost]
        public async Task<ActionResult> CreateYearBudget([FromBody] CreateYearBudgetCommand.Request model)
        {
            var createYearResult = await _mediator.Send(model);
            return Ok(createYearResult);
        }

        [HttpPost]
        public async Task<ActionResult> CreateMonthlyBudget([FromBody] CreateMonthlyBudgetCommand.Request model)
        {
            var createMonthlyBudgetResult = await _mediator.Send(model);
            return Ok(createMonthlyBudgetResult);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategoryMonthlyBudget([FromBody] CreateCategoryMonthlyBudgetCommand.Request model)
        {
            var CreateCategoryMonthlyBudgetResult = await _mediator.Send(model);
            return Ok(CreateCategoryMonthlyBudgetResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetBudgets([FromQuery] GetBudgetsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
