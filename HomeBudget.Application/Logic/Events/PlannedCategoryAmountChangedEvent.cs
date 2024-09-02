using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Events
{
    public class PlannedCategoryAmountChangedEvent : INotification
    {
        public required int Id { get; set; }

        public required int CategoryId { get; set; }

        public required decimal Amount { get; set; }
    }
}
