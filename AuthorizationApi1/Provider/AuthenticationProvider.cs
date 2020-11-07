using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApi1.Models;
using AuthorizationApi1.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationApi1.Provider
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthenticationProvider));
        private readonly IAuthenticationRepository _repository;
        public AuthenticationProvider(IAuthenticationRepository repository)
        {
            _log4net.Info("AuthenticationProvider constructor initiated.");
            _repository = repository;
        }

        /// <summary>
        /// Takes the token generated from repository and returns to controller.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string Login(User user)
        {
            string token = null;
            try
            {
                _log4net.Info("In Login method of Provider.");
                token = _repository.Login(user);
                _log4net.Info("Token string is returned to Controller.");
            }
            catch(Exception ex)
            {
                _log4net.Error("Exception found while delivering the token ="+ex.Message);
            }
            return token;
        }

    }
}
