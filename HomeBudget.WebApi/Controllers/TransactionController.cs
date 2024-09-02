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
            var DeleteTransactionResult = await _mediator.Send(model);
            return Ok(DeleteTransactionResult);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllTransactions([FromQuery] GetAllTransactionsQuery.Request model)
        {
            var data = await _mediator.Send(model);
            return Ok(data);
        }
    }
}
