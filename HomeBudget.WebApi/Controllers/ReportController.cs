using HomeBudget.Application.Logic.Budget.Category;
using HomeBudget.Application.Logic.Budget.Report;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class ReportController : BaseController
    {
        public ReportController(ILogger<TransactionController> logger, IMediator mediator) : base(logger, mediator)
        {
        }

        [HttpGet]
        public async Task<ActionResult> GetTransactions([FromQuery] GetTransactionsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}