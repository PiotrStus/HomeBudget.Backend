using HomeBudget.Application.Exceptions;

namespace HomeBudget.WebApi.Application.Response
{
    public class ValidationErrorResponse
    {

        public ValidationErrorResponse()
        {
        }

        // drugi konstruktor przyjmuje jako parametr nasz wyjatek
        // potrzebujemy jeszcze zrzutowac nasz rzucony wyjatek na ta nasza klase
        // mozna to zrobic konstruktorem, ktory jako parametr przyjmuje nasz
        // validation exception
        public ValidationErrorResponse(ValidationException validationException)
        {
            // lecimy sobie po błędach
            if (validationException != null)
            {
                if (validationException.Errors != null)
                {
                    // tworzymy sobie nasze klasy bledow
                    Errors = validationException.Errors.Select(e => new FieldValidationError()
                    { Error = e.Error,
                    FieldName = e.FieldName}).ToList(); 
                }
            };
        }



        public List<FieldValidationError> Errors { get; set; } = new List<FieldValidationError>();


        // znowu tutaj mamy swoja klase, ktora bedziemy zwracac
        // bo w standardowej moga byc jakies dodatkowe informacje, ktorych
        // nie chce zwracac uzytkownikowi na zewnatrz z api
        public class FieldValidationError
        {
            public required string FieldName { get; set; }

            public required string Error { get; set; }
        }
    }
}
