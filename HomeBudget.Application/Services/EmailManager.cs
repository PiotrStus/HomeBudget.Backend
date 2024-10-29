using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Services
{
    public class EmailManager
    {
        private readonly ITemplateProvider _templateProvider;
        private readonly ITemplateRenderer _templateRenderer;
        private readonly IEmailSender _emailSender;

        public EmailManager(ITemplateProvider templateProvider, ITemplateRenderer templateRenderer, IEmailSender emailSender)
        {
            _templateProvider = templateProvider;
            _templateRenderer = templateRenderer;
            _emailSender = emailSender;
        }

        public async Task SendEmail(string templateName, string to, object model)
        {
            var template = await _templateProvider.GetTemplateByName(templateName);

            if (template == null)
            {
                throw new ErrorException("EmailSendingError");
            }

            var body = _templateRenderer.FillTemplate(template.Body, model);

            var subject = _templateRenderer.FillTemplate(template.Subject, model);

            await _emailSender.SendEmail(to, subject, body);
        }
    }
}