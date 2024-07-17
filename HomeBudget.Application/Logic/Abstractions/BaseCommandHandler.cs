using HomeBudget.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Logic.Abstractions
{
    public abstract class BaseCommandHandler
    {
        protected readonly ICurrentAccountProvider _currentAccountProvider;
        protected readonly IApplicationDbContext _applicationDbContext;

        public BaseCommandHandler(ICurrentAccountProvider currentAccountProvider, IApplicationDbContext applicationDbContext)
        {
            _currentAccountProvider = currentAccountProvider;
            _applicationDbContext = applicationDbContext;
        }
    }
}
