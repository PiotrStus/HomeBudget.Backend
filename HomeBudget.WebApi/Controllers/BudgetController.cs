using HomeBudget.Application.Logic.Budget;
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
        public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryCommand.Request model)
        {
            var createCategoryResult = await _mediator.Send(model);
            return Ok(createCategoryResult);
        }

        [HttpPost]
        public async Task<ActionResult> CreateYearBudget([FromBody] CreateYearBudgetCommand.Request model)
        {
            var createYearResult = await _mediator.Send(model);
            return Ok(createYearResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

    }
}
