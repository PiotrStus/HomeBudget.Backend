using CacheManager.Core.Logging;
using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace HomeBudget.Application.Services
{
    public class LinkProvider : ILinkProvider
    {
        private readonly string? _baseUrl;
        private readonly string? _confirmActionRelativeUrl;

        public LinkProvider(IOptions<LinkProviderSettings> settings)
        {
            _baseUrl = settings.Value.BaseUrl?.TrimEnd('/');
            _confirmActionRelativeUrl = settings.Value.ConfirmActionRelativeUrl?.Trim('/');

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
                Path = $"{_confirmActionRelativeUrl}",
                //Query = $"guid={confirmationGuid}? "
            };
            string additionalText = "sd? ?df";

            // Łączenie GUID z dodatkowymi znakami
            string inputString = confirmationGuid + additionalText;
            //var test = "this is a test";
            string encodedGuid = Uri.EscapeDataString(confirmationGuid.ToString());
            //string encodedGuid = WebUtility.UrlEncode(confirmationGuid.ToString() + "sd? ?df");
            builder.Query = $"guid={encodedGuid}";

            return builder.ToString();
        }
    }
}