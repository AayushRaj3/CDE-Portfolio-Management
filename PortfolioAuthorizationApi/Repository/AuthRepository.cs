using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioAuthorizationApi.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace PortfolioAuthorizationApi.Repository
{
    
    public class AuthRepository : IAuthRepository
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthRepository));
        private IConfiguration _config;

        /// <summary>
        /// Static data for user model is created.
        /// </summary>
        private readonly Dictionary<int, string> users = new Dictionary<int, string>() {
            { 12345,"admin1"},
            {789,"admin2" }
        };


        /// <summary>
        /// Constructor of AuthRepository which initializes IConfiguration Object.
        /// </summary>
        /// <param name="config"></param>
        public AuthRepository(IConfiguration config)
        {
            _log4net.Info("AuthenticationRepository constructor initiated.");
            _config = config;
        }

        /// <summary>
        ///  Generates the token if the user credentials are correct.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

        public string GenerateToken(User user)
        {
            try
            {
                _log4net.Info("Token generation started");
                if (!users.Any(u => u.Key == user.PortFolioID && u.Value == user.Password))
                {
                    _log4net.Info("User Credentials are not correct so token cannot be generated.");
                    return null;
                }
                _log4net.Info("User Credentials are correct.");
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.PortFolioID.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                _log4net.Info("Token is generated successfully.");
                return tokenHandler.WriteToken(token);
            }
            catch(Exception exception)
            {
                _log4net.Error("Exception found while generating the token " + exception.Message);
                return null;
            }

        }
    }
}
