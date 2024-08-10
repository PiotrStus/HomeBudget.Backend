using HomeBudget.Application.Logic.Budget.Category;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        public CategoryController(ILogger<CategoryController> logger, IMediator mediator) : base(logger, mediator)
        {
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
        public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryCommand.Request model)
        {
            var createCategoryResult = await _mediator.Send(model);
            return Ok(createCategoryResult);
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

        [HttpGet]
        public async Task<ActionResult> GetPlannedCategories([FromQuery] GetPlannedCategoriesQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

    }
}
