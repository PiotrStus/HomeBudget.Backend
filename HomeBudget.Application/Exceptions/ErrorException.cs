using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageMonitor.Application.Exceptions
{
    public class ErrorException : Exception
    {
        // property z błedem
        public string Error { get; private set; }

        // konstruktor z parametrem bledu, zeby latwiej bylo tworzyc takie klasy wyjatkow
        public ErrorException(string error)
        {
            Error = error;
        }
    }
}
