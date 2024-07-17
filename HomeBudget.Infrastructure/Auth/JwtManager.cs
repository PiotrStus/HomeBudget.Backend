using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Auth
{
    public class JwtManager
    {
        private readonly JwtAuthenticationOptions _jwtOptions;

        // tworzymy stala, ktora bedzie przechowywac nazwe claima, czyli ID uzytkownika
        // robimy to po to, zeby miec w jednym miejscu taki napis, zeby go nie powielac w kilku miejsach
        public const string UserIdClaim = "UserId";


        // interfejs IOptions -> standardowy interfejs z DI z ASP.NET Core
        public JwtManager(IOptions<JwtAuthenticationOptions> jwtOptions)
        {
            // poprzez Value dostajemy się do klasy JwtAuthenticationOptions
            _jwtOptions = jwtOptions.Value;
        }

        // pierwsza metoda prywatna do wygenerowania klucza na podstawie Secret z ustawien
        // uzywamy metod wbudowanych w framework, zeby nie pisac ich samemu, bo to moze byc niebezpieczne
        private SecurityKey GetSecurityKey()
        {
            // sprawdzenie czy secret zostal ustawiony, jesli nie to wyrzucamy wyjatek
            if (string.IsNullOrWhiteSpace(_jwtOptions.Secret))
            {
                throw new ArgumentException("JWT options secret is empty!");
            }
            // jesli tak to po prostu go tworzymy odczytujac byte z Secreta ustawien
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret));
        }

        // kolejna metoda, ktora wygeneruje token z zadanym claimem
        // claimy to mechanizm wbudowany, ktory pozwala przechowywac rozne informacje
        // o uzytkowniku miedzy innymi w tokenie

        // pozniej uzupelnimy claims, wykorzystujac ID uzytkownika
        // dzieki temu ten ID uzytkownika bedzie zaszyty w srodku tokena
        private string GenerateTokenWithClaims(IEnumerable<Claim> claims)
        {

            // pierwsze co robimy to tworzymy klucz
            var mySecurityKey = GetSecurityKey();

            // tworzymy tokenHandler z dokumentacji
            var tokenHandler = new JwtSecurityTokenHandler();
            // tworzymy tokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // w parametrze subject mamy, gdzie przekazujemy claimy, ktore chcemy miec w tokenie
                Subject = new ClaimsIdentity(claims),
                // data wygasniecia bierzemy aktualna date i dodajemy z ustawien
                Expires = DateTime.UtcNow.AddDays(_jwtOptions.ExpireInDays),
                // wystawca
                Issuer = _jwtOptions.Issuer,
                // odbiorca
                Audience = _jwtOptions.Audience,
                // credentials uzyte do podpisania tokena
                // uzywamy klucza i wybieramy algorytm hashowania, tutaj: Sha256
                SigningCredentials = new SigningCredentials(mySecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        // dodajemy metoda, ktora na podstawie ID uzytkownika wygeneruje token
        // zaszyfrowany
        public string GenerateUserToken(int userId)
        {
            var claims = new Claim[]
                {
                    new Claim(UserIdClaim, userId.ToString()),
                };

            return GenerateTokenWithClaims(claims);
        }

        // metoda potrzebna do walidacji tokena
        // w parametrze przyjmuje caly token
        public bool ValidateToken(string token)
        {
            // jesli jest pusty to zwroci false
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            // logika do deserializacji i walidacji

            // wyciagamy klucz
            var mySecurityKey = GetSecurityKey();

            // tworzymy tokenHandler
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                // podajemy ten token jako parametr i parametry jakie chcemy walidowac
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    // walidujemy:

                    // podpis
                    ValidateIssuerSigningKey = true,
                    // wystawce
                    ValidateIssuer = true,
                    // odbiorcow
                    ValidateAudience = true,
                    // czy nie wygasl w czasie
                    ValidateLifetime = true,
                    // jako parametry przekazujemy tego wystawce i odbircow
                    // z naszych ustawien, dzieki czemu ta walidacja porowna to co jest w ustawieniach
                    // z tym co przekazemy
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidAudience = _jwtOptions.Audience,
                    // klucz, ktory sluzy do podpisania tokenu i tym kluczem zwalidujemy
                    // autentycznosc i waznosc tego tokenu 
                    IssuerSigningKey = mySecurityKey
                    // i w odpowiedzi dostajemy tutaj ten token jako efekt uboczny
                    // my tego nie bedziemy potrzebowac
                    // my potrzebujemy true albo false
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        // metoda potrzebna do wyciagniecia id uzytkownika z tokena, ktory przyjdzie w ciastku
        public string? GetClaim(string token, string claimType)
        {
            // ten sam handler, z dokumentacji, wbudowany
            var tokenHandler = new JwtSecurityTokenHandler();
            // uzycie ReadToken, czyli odczytujemy token ze stringa i rzutujemy na JwtSecurityToken
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            // jesli nie uda sie go odczytac to zwracamy nulla
            if (securityToken == null)
            {
                return null;
            }
            // jesli sie uda to on ma parametr Claims, wyszukujemy po parametrze podanym claimType
            var stringClaimValue = securityToken.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
            // jesli sie znajduje w tokenie to jest zwracane
            return stringClaimValue;
        }
    }
}
