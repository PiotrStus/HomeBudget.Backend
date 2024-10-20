using DotLiquid;
using HomeBudget.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Email.Templates
{
    public class TemplateRenderer : ITemplateRenderer
    {
        public string FillTemplate(string template, object model)
        {
            var parsedTemplate = DotLiquid.Template.Parse(template);
            return parsedTemplate.Render(Hash.FromAnonymousObject(model));
        }
    }
}