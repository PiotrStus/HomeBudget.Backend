using HomeBudget.Application.Exceptions;
using HomeBudget.Application.Interfaces;
using HomeBudget.Application.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Email.Template
{
    public class TemplateProvider : ITemplateProvider
    {
        public TemplateProvider()
        {
        }

        public Task<EmailTemplate?> GetTemplateByName(string name)
        {
            var templates = EmailTemplates.Templates;
            if (templates.TryGetValue(name, out var template))
            {
                return Task.FromResult<EmailTemplate?>(template);
            }

            return Task.FromResult<EmailTemplate?>(null);
        }
    }
}