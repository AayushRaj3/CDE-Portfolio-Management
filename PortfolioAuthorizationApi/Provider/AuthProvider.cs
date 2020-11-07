using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioAuthorizationApi.Models;
using PortfolioAuthorizationApi.Repository;

namespace PortfolioAuthorizationApi.Provider
{
    public class AuthProvider:IAuthProvider
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthProvider));
        private readonly IAuthRepository _authRepository;


        /// <summary>
        /// Constructor of provider that initalizes the repository object.
        /// </summary>
        /// <param name="authRepository"></param>
        public AuthProvider(IAuthRepository authRepository)
        {
            _log4net.Info("AuthProvider constructor initiated.");
            _authRepository = authRepository;
        }


        /// <summary>
        /// Calls repository function to generate token and deliver it to controller.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string AuthenticateUser(User user)
        {
            string token = null;
            try
            {
                _log4net.Info("In AuthenticateUser method of the provider.Repository GenerateToken function is called.");
                token = _authRepository.GenerateToken(user);
                _log4net.Info("Token string is returned to the Controller.");
            }
            catch(Exception exception)
            {
                _log4net.Error("Exception found while returning the token to controller =" + exception.Message);
            }
            return token;
        }
    }
}
