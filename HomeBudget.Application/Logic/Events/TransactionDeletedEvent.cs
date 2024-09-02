using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Events
{
    public class TransactionDeletedEvent : INotification
    {
        public required int TransactionId { get; set; }
    }
}
