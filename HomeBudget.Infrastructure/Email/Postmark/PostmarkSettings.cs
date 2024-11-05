using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Email.Postmark
{
    public class PostmarkSettings
    {
        public const string From = "admin@finbudget.pl";
        public string? MessageStream { get; set; }

        public string? ApiToken { get; set; }

        public string? ApiUrl { get; set; }
    }
}