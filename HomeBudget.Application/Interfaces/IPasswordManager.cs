using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Application.Interfaces
{
    public interface IPasswordManager
    {
        // metoda do tworzenia hasha hasel
        string HashPassword(string password);

        // metoda do weryfikacji tego hasha
        // parametry: hash zapisany w bazie oraz wprowadzone haslo
        // dzieki temu ta metoda bedzie mogla porownac
        // wyliczyc hasha na nowo z wprowadzonego hasla i
        // porownac go z hashem z bazy danych
        bool VerifyPassword(string hash, string password);
    }
}
