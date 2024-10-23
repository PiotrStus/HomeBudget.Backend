using HomeBudget.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Services
{
    public class LinkProvider : ILinkProvider
    {
        private readonly string? _baseUrl;
        private readonly string? _confirmActionRelativeUrl;

        public LinkProvider(IOptions<LinkProviderSettings> settings)
        {
            _baseUrl = settings.Value.BaseUrl;
            _confirmActionRelativeUrl = settings.Value.ConfirmActionRelativeUrl;
        }

        public string GenerateConfirmationLink(Guid confirmationGuid)
        {
            var confirmationLink = $"{_baseUrl}{_confirmActionRelativeUrl}/?guid={confirmationGuid}";

            return confirmationLink ;
        }
    }
}
