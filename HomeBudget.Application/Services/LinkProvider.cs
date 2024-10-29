using CacheManager.Core.Logging;
using HomeBudget.Application.Exceptions;
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

            if (string.IsNullOrWhiteSpace(_confirmActionRelativeUrl))
            {
                throw new ArgumentException("ConfirmActionRelativeUrl cannot be null or empty.", nameof(settings));
            }
        }

        public string GenerateConfirmationLink(Guid confirmationGuid)
        {
            //var confirmationLink = $"{_baseUrl}{_confirmActionRelativeUrl}/?guid={confirmationGuid}";
            
            var builder = new UriBuilder(_baseUrl!)
            {
                Path = $"{_confirmActionRelativeUrl}/",
                Query = $"guid={confirmationGuid}"
            };


            return builder.Uri.ToString();
        }
    }
}