using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.EventHandlers.TransactionCreated
{
    public class TransactionCreatedEvent : INotification
    {
        public int TransactionId { get; set; }

        public int CategoryId { get; set; }

        public int AccountId { get; set; }

        public string Name { get; set; } = default!;

        public DateTimeOffset Date { get; set; }

        public decimal Amount { get; set; }
    }
}
