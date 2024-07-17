using Microsoft.AspNetCore.Identity;
using HomeBudget.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Infrastructure.Auth
{
    public class PasswordManager : IPasswordManager
    {

        private readonly IPasswordHasher<DummyUser> _passwordHasher;

        // w środku potrzebujemy zależnosci do klasy hashujacej haslo
        // wiec dodajemy sobie konstruktor
        public PasswordManager(IPasswordHasher <DummyUser> passwordHasher) 
        {
            _passwordHasher = passwordHasher;
        }

        // to jest ta metoda z nugeta do hashowania
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(new DummyUser(), password);
        }

        // metoda do weryfikacji
        public bool VerifyPassword(string hash, string password)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(new DummyUser(), hash, password);
            // w rezultacie otrzymujemy enuma
            if (verificationResult == PasswordVerificationResult.Success || verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return true;
            }

            return false;
        }

        // klasa dummyuser jest po to
        // poniewaz ten parametr passwordHasher wymaga parametru generycznego
        // ktory jest uzytkownikiem dla ktorego hashujemy to haslo
        // ona symuluje tego usera
        public class DummyUser
        {

        }
    }
}
