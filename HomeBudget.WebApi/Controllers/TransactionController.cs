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

        [HttpPost]
        public async Task<ActionResult> UpdateTransaction([FromBody] UpdateTransactionCommand.Request model)
        {
            var updateTransactionResult = await _mediator.Send(model);
            return Ok(updateTransactionResult);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTransaction([FromBody] DeleteTransactionCommand.Request model)
        {
            var deleteTransactionResult = await _mediator.Send(model);
            return Ok(deleteTransactionResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetTransactions([FromQuery] GetTransactionsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }

        [HttpGet]
        public async Task<ActionResult> GetTransaction([FromQuery] GetTransactionQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
