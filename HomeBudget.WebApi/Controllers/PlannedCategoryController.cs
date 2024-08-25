using HomeBudget.Application.Logic.Budget.Category;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class PlannedCategoryController : BaseController
    {
        public PlannedCategoryController(ILogger<CategoryController> logger, IMediator mediator) : base(logger, mediator)
        {
        }

        [HttpPost]
        public async Task<ActionResult> CreatePlannedCategory([FromBody] CreatePlannedCategoryCommand.Request model)
        {
            var createPlannedCategoryResult = await _mediator.Send(model);
            return Ok(createPlannedCategoryResult);
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePlannedCategory([FromBody] UpdatePlannedCategoryCommand.Request model)
        {
            var UpdatePlannedCategoryResult = await _mediator.Send(model);
            return Ok(UpdatePlannedCategoryResult);
        }

        [HttpPost]
        public async Task<ActionResult> DeletePlannedCategory([FromBody] DeletePlannedCategoryCommand.Request model)
        {
            var DeletePlannedCategoryResult = await _mediator.Send(model);
            return Ok(DeletePlannedCategoryResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPlannedCategories([FromQuery] GetPlannedCategoriesQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetPlannedCategory([FromQuery] GetPlannedCategoryQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
