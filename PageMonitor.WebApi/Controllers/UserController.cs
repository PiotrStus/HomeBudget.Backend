﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PageMonitor.Application.Logic.User;
using PageMonitor.Infrastructure.Auth;
using PageMonitor.WebApi.Application.Auth;
using PageMonitor.WebApi.Application.Response;

namespace PageMonitor.WebApi.Controllers
{

    // zmienamy routing w tej chwili dzialat tak, ze mamy adres api
    // a nastepnie adres kontrollera
    //[Route("api/[controller]")]
    // natomiast my jeszcze chcemy adres akcji dlatego tez
    // wiec teraz nie ma prefiksu api
    // nestepnie jest nazwa kontrolera i akcji ktora wywolujemy
    [Route("[controller]/[action]")]
    [ApiController]
    //dzieiczy po BaseControllerze
    public class UserController : BaseController
    {
        private readonly JwtManager _jwtManager;
        private readonly CookieSettings? _cookieSettings;

        // dodajemy 2 properties w konstruktorze, ktora zostana wstrzykniete przez DI
        // IOptions<CookieSettings> cookieSettings => jedna z nich jest ustawienie ciastek,
        // ponieważ w tym kontrolerze bedziemy tez ustawiac ciastko z tokenem jwt
        // klasa jwtManager pozwala nam utworzyc token JWT
        // te wszystkie klasy powinny byc zarejesrowane w kontenerze DI
        public UserController(ILogger<UserController> logger,
            IOptions<CookieSettings> cookieSettings,
            JwtManager jwtManager,
            IMediator mediator) : base(logger, mediator)
        {
            _jwtManager = jwtManager;
            // jesli sa ustawienia to przypisujemy wartosc a jesli nie to null
            _cookieSettings = cookieSettings != null ? cookieSettings.Value : null;
        }

        // dodajemy akcje, ktora wywola naszego commanda
        // akcja Create typu POST tworzącą nowe konto w aplikacji
        // dobrze, zeby nazwa odpowiadala nazwa commanda
        // w parametrze jest request, czyli nasz request z commanda
        // zawartosc dane do tej klasy sa odczytywane z body requesta
        // jest to requesta typu post
        [HttpPost]
        public async Task<ActionResult> CreateUserWithAccount([FromBody] CreateUserWithAccountCommand.Request model)
        {
            // to co robimy to bierzemy mediatora i wysylamy obiekt o nazwie model (nasz request)
            // mediator zajmie sie wyslaniem tego i przekazaniem do handlera
            // ktory znajduje sie w klasie CreateUserWithAccountCommand
            // w projekcie Application
            var createAccountResult = await _mediator.Send(model);
            // request do zakladania konta tez zwraca UserId
            var token = _jwtManager.GenerateUserToken(createAccountResult.UserId);
            SetTokenCookie(token);
            return Ok(new JwtToken() { AccessToken = token });
        }

        [HttpPost]
        // parametry beda odczytywane z body
        // model to jest nasz logincommand request z wartsty application
        public async Task<ActionResult> Login([FromBody] LoginCommand.Request model)
        {
            // wysylamy przez mediatora tego commanda
            // dostaniemy jakis rezultat
            var loginResult = await _mediator.Send(model);
            // generujemy token na podstawie id usera
            // UserId to jest to co nam zwraca LoginCommand w Result
            var token = _jwtManager.GenerateUserToken(loginResult.UserId);
            // jesli mamy ten token to zwracamy ciastko
            SetTokenCookie(token);
            // zwracanie respona w formacie json obiektu jwttoken, ktory zawiera accesstoken
            return Ok(new JwtToken() { AccessToken = token });
        }



        // dodajemy metode, która ustawi ciastko z tokenem
        // w parametrze przyjmuje token
        private void SetTokenCookie(string token)
        {
            // ustawienia ciastka z frameworka
            // to sa domyslne
            var cookieOption = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                // 30 dni data wygasniecia
                Expires = DateTime.Now.AddDays(30),
                SameSite = SameSiteMode.Lax,
            };

            // natomiast jesli mamy, to
            if (_cookieSettings != null)
            {
                
                cookieOption = new CookieOptions()
                {
                    HttpOnly = cookieOption.HttpOnly,
                    Expires = cookieOption.Expires,
                    // bierzemy z ustawien Secure i SamSite
                    Secure = _cookieSettings.Secure,
                    SameSite = _cookieSettings.SameSite,
                };
            }

            // ktore przychodza jako parametr do ustawienia w repsonse
            Response.Cookies.Append(CookieSettings.CookieName, token, cookieOption);
        }



    }
}
