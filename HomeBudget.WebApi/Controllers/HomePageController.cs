using MediatR;
using Microsoft.AspNetCore.Mvc;
using HomeBudget.Application.Logic.Budget.HomePage;
using HomeBudget.Application.Logic.Budget.Transaction;

namespace HomeBudget.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class HomePageController : BaseController
    {
        public HomePageController(ILogger<CategoryController> logger, IMediator mediator) : base(logger, mediator)
        {
        }


        [HttpGet]
        public async Task<ActionResult> GetPlannedMonthlyCategories([FromQuery] GetPlannedMonthlyCategoriesQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetMonthlyBalance([FromQuery] GetMonthlyBalanceQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> CheckBudgetExists([FromQuery] CheckBudgetQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetRecentTransaction([FromQuery] GetRecentTransactionsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
