using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Domain.Entities
{
    public class AccountUser: DomainEntity
    {
        public int AccountId { get; set; }

        public Account Account { get; set; } = default!;

        public int UserId {  get; set; }

        public User User { get; set; } = default!;

        public required bool IsAdmin { get; set; }
    }
}
