using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PortfolioAuthorizationApi.Provider;
using PortfolioAuthorizationApi.Models;

namespace PortfolioAuthorizationApi.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthController));
        private readonly IAuthProvider _authProvider;

        /// <summary>
        /// Constructor of the controller that initializes the provider object.
        /// </summary>
        /// <param name="authProvider"></param>
        public AuthController(IAuthProvider authProvider)
        {
            _log4net.Info("AuthController constructor initiated.");
            _authProvider = authProvider;
        }

        /// <summary>
        /// Checks whether the user is authorized or not.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AuthenticateUser(User user)
        {
            _log4net.Info("AuthController AuthenticateUser method initiated.");
            try
            {
                var token = _authProvider.AuthenticateUser(user);
                if (string.IsNullOrEmpty(token))
                {
                    _log4net.Info("Unauthorized User.");
                    return Unauthorized();
                }
                _log4net.Info("Authorized User.");
                return Ok(new { tokenString = token });
            }
            catch(Exception exception)
            {
                _log4net.Error("Exception found while authenticating the user=" + exception.Message);
                return new StatusCodeResult(500);

            }
        }
    }
}
