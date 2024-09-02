using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Events
{
    public interface ITransactionEvent : INotification
    {
        int CategoryId { get; set; }

        int TransactionId { get; set; }
    }
}
