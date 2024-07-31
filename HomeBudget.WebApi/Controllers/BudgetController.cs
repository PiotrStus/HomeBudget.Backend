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
        public async Task<ActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetCategory([FromQuery] GetCommandQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateCategory([FromBody] UpdateCategoryCommand.Request model)
        {
            var UpdateCategoryResult = await _mediator.Send(model);
            return Ok(UpdateCategoryResult);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCategory([FromBody] DeleteCategoryCommand.Request model)
        {
            var DeleteCategoryResult = await _mediator.Send(model);
            return Ok(DeleteCategoryResult);
        }

    }
}
