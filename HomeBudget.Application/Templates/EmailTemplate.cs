using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Templates
{
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