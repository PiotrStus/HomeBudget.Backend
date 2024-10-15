using HomeBudget.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomeBudget.Domain.Enums;

namespace HomeBudget.Domain.Entities
{
    public class UserConfirmGuid : DomainEntity
    {
        public int UserId { get; set; }

        public User User { get; set; } = default!;

        public Guid UserGuid { get; set; }

        public UserGuidType GuidType { get; set; }
    }
}
