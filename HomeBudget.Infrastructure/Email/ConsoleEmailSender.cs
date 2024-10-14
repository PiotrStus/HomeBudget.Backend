using CacheManager.Core.Logging;
using HomeBudget.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Email
{
    public class ConsoleEmailSender : IEmailSender
    {
        private readonly ILogger<ConsoleEmailSender> _logger;

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            _logger = logger;
        }

        public Task SendEmail(string to, string subject, string body)
        {
            _logger.LogInformation("------ Email Sender ------");
            _logger.LogInformation($"To: {to}");
            _logger.LogInformation($"Subject: {subject}");
            _logger.LogInformation($"Body: {body}");
            _logger.LogInformation("--------------------------");
            return Task.CompletedTask;
        }
    }
}
