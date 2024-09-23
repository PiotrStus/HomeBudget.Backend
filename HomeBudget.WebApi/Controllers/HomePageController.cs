using MediatR;
using Microsoft.AspNetCore.Mvc;
using HomeBudget.Application.Logic.Budget.HomePage;

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
        public async Task<ActionResult> GetMonthlyBalanceQuery([FromQuery] GetMonthlyBalanceQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
