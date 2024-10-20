using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Templates
{
    public static class EmailTemplates
    {
        private static readonly Dictionary<string, EmailTemplate> _templates = new()
        {
            {
                "confirmAccount", new EmailTemplate(
                    @"<h3>Witaj, {{ username }}!</h3>
                      <p><a href='{{ confirmationLink }}'>Kliknij tutaj, aby potwierdzić aktywację konta</a></p>",
                    "Potwierdzenie aktywacji konta"
                )
            }
        };
        public static IReadOnlyDictionary<string, EmailTemplate> Templates => _templates;
        public class EmailTemplate
        {
            public string Body { get; set; }
            public string Subject { get; set; }
            public EmailTemplate(string body, string subject)
            {
                Body = body;
                Subject = subject;
            }
        }
    }
}