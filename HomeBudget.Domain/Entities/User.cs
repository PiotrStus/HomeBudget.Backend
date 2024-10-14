using HomeBudget.Domain.Common;
using HomeBudget.Domain.Entities.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities
{
    public class User : DomainEntity
    {
        public required string Email { get; set; }

        public required bool IsActivated { get; set; }

        public required string HashedPassword { get; set; }

        public DateTimeOffset RegisterDate { get; set; }

        public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
