using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApi1.Models;
using AuthorizationApi1.Provider;
using AuthorizationApi1.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationApi1.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthController));
        private readonly IAuthenticationProvider _provider;

        public AuthController(IAuthenticationProvider provider)
        {
            _log4net.Info("AuthController constructor initiated.");
            _provider = provider;
        }

        /// <summary>
        /// Checks whether the user is authenticated or not.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        [HttpPost]
        public IActionResult AuthenticateUser(User user)
        {
            _log4net.Info("AuthController AuthenticateUser method initiated.");
            try
            {
                IActionResult response = Unauthorized();
                var token = _provider.Login(user);

                if (token != null)
                {
                    _log4net.Info("Token is received.");
                    response = Ok(new { tokenString = token });
                }
                else
                {
                    _log4net.Info("Token is not received.");
                }
                _log4net.Info("Response is given, user is authorized or unauthorized.");
                return response;   
            }
            catch(Exception exception)
            {
                _log4net.Error("Exception found while authenticating the user="+exception.Message);
                return new StatusCodeResult(500);
            }
        }
        

        


    }
}
