namespace PageMonitor.WebApi.Application.Auth
{
    public class CookieSettings
    {
        // dodajemy stałą z nazwą ciastka, ponieważ będziemy używać tej stałej
        // w różnych miejsach i dobrą praktyką jest wrzucenie tego jaką stałą, żeby
        // nie powielać tego napisu
        public const string CookieName = "auth.token";

        // ustawienie Secure, ktore bedzie przydatne ustawienie go na false,
        // kiedy bedziemy lokalnie testowac nasza aplikacje, poniewaz 
        // nie mamy wlaczonego https, wiec wszystkie ciastka beda mialy wlaczona
        // wlasciwosc cyklu na false
        public bool Secure { get; set; } = true;


        // to do dokumentacji, więcej o tym
        public SameSiteMode SameSite { get; set; } = SameSiteMode.Lax;
    }
}
