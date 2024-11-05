using HomeBudget.Application.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace HomeBudget.Infrastructure.Email.Postmark
{
    public class PostmarkEmailSender : IEmailSender
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly PostmarkSettings _postmarkSettings;

        public PostmarkEmailSender(ILogger<PostmarkEmailSender> logger,
            IHttpClientFactory clientFactory,
            IOptions<PostmarkSettings> postmarkSettings)
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _postmarkSettings = postmarkSettings.Value;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            var httpClient = _clientFactory.CreateClient(PostmarkConfiguration.HTTP_CLIENT_NAME);

            var values = new Dictionary<string, object>();

            values.Add("MessageStream", _postmarkSettings.MessageStream!);

            values.Add("To", String.Join(',', to));

            values.Add("From", PostmarkSettings.From);

            values.Add("Subject", subject);

            values.Add("HtmlBody", body);

            var response = await httpClient.PostAsync("/email", JsonContent.Create(values));

            await ThrowExceptionOnDataError(response);

            response.EnsureSuccessStatusCode();
        }

        private async Task ThrowExceptionOnDataError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error from Postmark API: {response.StatusCode}, Details: {errorContent}");
            }
        }
    }
}