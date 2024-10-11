using HomeBudget.Application.Interfaces;
using HomeBudget.Infrastructure.Auth;

namespace HomeBudget.WebApi.Application.Auth
{
    public class JwtAuthenticationDataProvider : IAuthenticationDataProvider
    {
        private readonly JwtManager _jwtManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // do tej klasy wstrzykiwane są dwie zależności
        // jwtmanager, ktory pozwala na tworzenie i czytanie tokenow jwt
        // oraz httpcontextaccasesor, ktory pozwoli nam dostac sie do szczegolow naszego
        // requesta http
        public JwtAuthenticationDataProvider(JwtManager jwtManager, IHttpContextAccessor httpContextAccessor)
        {
            _jwtManager = jwtManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetTokenFromCookie()
        {
            // co ona robi?
            // odwoluje sie do http contextu, czyli do aktualnego requesta
            // on ma kolekcje cookies, nastepnie wyciagamy ciastko o nazwie
            // zdefiniowanej w stalej , czyli u nas to bedzie "auth.token";
            // powtazanie kodu jest zla praktyka i dlatego ta stala
            return _httpContextAccessor.HttpContext?.Request.Cookies[CookieSettings.CookieJWTName];
        }


        // odczytywanie tego samego tokena z naglowka odpowiedzi
        // poniewaz bedziemy tez obslugiwac taki mechanizm standardowy
        // w ktorym token przychodzi w naglowku, dzieki czemu bedzie latwiej
        // testowac np. w postamanie
        private string? GetTokenFromHeader()
        {
            // wyciagamy naglowek rzadania o nazwie Authorization
            var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
            // sprawdzamy czy nie jest pusty
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }
            // jesli nie jest pusty to go rozdzielamy po spacji
            var splited = authorizationHeader.Split(' ');
            // sprawdzamy czy pierwszym elementem jest taki ciag znakow
            // to jest wg. standardu sposob przekazywania tokena - "Bearer"
            // wlasnie uzycie takiego slowa kluczowego a nastepnie po spacji tokena
            // wtedy w splited[1] mamy nasz token i sobie go zwracamy z naszej metody
            if (splited.Length > 1 && splited[0] == "Bearer")
            {
                return splited[1];
            }

            return null;
        }

        // wyciaganie konkretnej wartosci podanego claima
        private string? GetClaimValue(string claimType)
        {

            // najpierw wyciagamy token
            var token = GetTokenFromHeader();

            // jesli on jest pusty to nastepnie wyciagamy token z ciastka
            // wiec jesli mamy token w naglowku i ciastku to ten w naglowku
            // ma pierwszenstwo 
            if (string.IsNullOrEmpty(token))
            {
                token = GetTokenFromCookie();
            }


            // nastepnie sprawdzamy czy ten token nie jest pusty
            // i go walidujemy, wykorzystujac klase jwt manager, ktora
            // zaimplementowalismy w poprzedniej lekcji
            if (!string.IsNullOrWhiteSpace(token) && _jwtManager.ValidateToken(token))
            {
                // nastepnie z tokena wyciagamy nasz konkretny claim, ktory tutaj przychodzi
                // w naszym przypadku bedzie to user id
                return _jwtManager.GetClaim(token, claimType);
            }

            return null;
        }

        public int? GetUserId()
        {
            // wyciagamy sobie claimvalue = userid
            // znowu uzywamy stalej, ktora jest zdefiniowana w jwtmanagerze
            // i to bedzie id uzytkownika zapisany jako string
            var userIdString = GetClaimValue(JwtManager.UserIdClaim);
            // nstepnie probujemy sparsowac to id uzytkownika na inta
            // jesli to sie uda to zwracamy userid
            // jesli nie to zwracamy nulla
            if (int.TryParse(userIdString, out int res))
            {
                return res;
            }

            return null;
        }

        public int? GetAccountId()
        {
            var accountIdString = _httpContextAccessor.HttpContext?.Request.Cookies[CookieSettings.CookieAccountName];

            if (int.TryParse(accountIdString, out var res))
            {
                return res;
            }

            return null;
        }
    }
}
