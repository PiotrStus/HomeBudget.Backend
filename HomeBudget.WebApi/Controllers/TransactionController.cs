using HomeBudget.Application.Logic.Budget.Category;
using HomeBudget.Application.Logic.Budget.Transaction;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeBudget.WebApi.Controllers
{

    [Route("[controller]/[action]")]
    [ApiController]
    public class TransactionController : BaseController
    {
        public TransactionController(ILogger<TransactionController> logger, IMediator mediator) : base(logger, mediator)
        {
        }


        [HttpPost]
        public async Task<ActionResult> CreateTransaction([FromBody] CreateTransactionCommand.Request model)
        {
            var createTransactionResult = await _mediator.Send(model);
            return Ok(createTransactionResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllTransactions([FromQuery] GetAllTransactionsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
