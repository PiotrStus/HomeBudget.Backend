using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class TransactionData
    {
        public required int TransactionId { get; set; }

        public required int CategoryId { get; set; }

        public required int AccountId { get; set; }

        public required DateTimeOffset Date { get; set; }

        public required decimal Amount { get; set; }
    }
}
