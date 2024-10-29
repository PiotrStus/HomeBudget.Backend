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
            _baseUrl = settings.Value.BaseUrl?.TrimEnd('/');
            _confirmActionRelativeUrl = settings.Value.ConfirmActionRelativeUrl?.TrimStart('/');
        }

        public string GenerateConfirmationLink(Guid confirmationGuid)
        {
            //var confirmationLink = $"{_baseUrl}{_confirmActionRelativeUrl}/?guid={confirmationGuid}";

            var builder = new UriBuilder(_baseUrl!)
            {
                Path = $"{_confirmActionRelativeUrl}",
                Query = $"guid={confirmationGuid}"
            };

            return builder.Uri.ToString();
        }
    }
}