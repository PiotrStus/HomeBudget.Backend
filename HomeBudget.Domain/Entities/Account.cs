using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Domain.Entities
{
    public class Account : DomainEntity
    {
        public required string Name { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public ICollection<AccountUser> AccountUsers { get; set; } = new List<AccountUser>();
    }
}
