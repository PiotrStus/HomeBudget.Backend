using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Auth
{
    public class JwtAuthenticationOptions
    {
        // hasło, które służy do szyfrowania/deszyfrowania tokena
        public string? Secret { get; set; }
        // dostawca tokena
        public string? Issuer { get; set; }
        // kto jest odbiorcą tokena
        public string? Audience { get; set; }
        // czas wygaśnięcia
        public int ExpireInDays { get; set; } = 30;
    }
}
