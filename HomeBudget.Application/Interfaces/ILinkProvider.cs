using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Interfaces
{
    public interface ILinkProvider
    {
        string GenerateConfirmationLink(Guid confirmationGuid);
    }
}
