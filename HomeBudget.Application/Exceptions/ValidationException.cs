using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Exceptions
{
    public class ValidationException : Exception
    {
        // lista błędów
        public List<FieldError> Errors { get; set; } = new List<FieldError>();

        // klasa pomocnicza, która będzie zawierać informacje o tych błędach
        // nie towrzymy jej w osobnych plikow, zeby nie powielac
        // niewiadomo ile plików
        // ona jest wykorzystywana tylko w tej klasie
        // wiec moze byc jako klasa zagniezdzona
        public class FieldError
        {
            public required string FieldName { get; set; }

            public required string Error { get; set; }
        }
    }
}
