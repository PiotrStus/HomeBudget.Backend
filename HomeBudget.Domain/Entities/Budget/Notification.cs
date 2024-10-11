using HomeBudget.Domain.Common;
using HomeBudget.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities.Budget
{
    public class Notification : DomainEntity
    {
        public required DateTimeOffset Date { get; set; }

        public required int UserId { get; set; }

        public required int AccountId { get; set; }

        public required string Content { get; set; }

        public required bool IsRead { get; set; }

        public required NotificationType NotificationType { get; set; }

        public required string CategoryName { get; set; }

        public User User { get; set; } = default!;

        public Account Account { get; set; } = default!;
    }
}
