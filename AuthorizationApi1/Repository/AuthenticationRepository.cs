using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthorizationApi1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationApi1.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthenticationRepository));
        private readonly IConfiguration _config;
        private readonly Dictionary<int, string> users = new Dictionary<int, string>()
        {
            { 12345,"admin1" },
            { 789,"admin2" }
        };
        
        public AuthenticationRepository(IConfiguration config)
        {
            _log4net.Info("AuthenticationRepository constructor initiated.");
            _config = config;
        }

        /// <summary>
        /// Generates the token if the user credentials are correct.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>

       public string GenerateJSONWebToken(User user)
        {
            try
            {
                _log4net.Info("Token generation started");
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                List<Claim> claims = new List<Claim>() {
                new Claim(JwtRegisteredClaimNames.Sub, user.PortFolioID.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.PortFolioID.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                  _config["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddMinutes(30),
                  signingCredentials: credentials);
                _log4net.Info("Token generation completed");
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch(Exception ex)
            {
                _log4net.Error("Exception found while generating the token " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Checks whether the user credenticals are valid or not
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string Login(User user)
        {
            try
            {
              _log4net.Info("Repository Login method initiated");
              if (users.Any(u => u.Key == user.PortFolioID && u.Value == user.Password))
              {
                    _log4net.Info("User Credentials are validated");
                    var tokenString = GenerateJSONWebToken(user);
                    _log4net.Info("Token string is returned.");
                    return tokenString;
                }
                    _log4net.Info("User credentials are wrong.");
                    return null;
                }
                catch (Exception ex)
                {
                    _log4net.Error("Exception found while validating the user = " + ex.Message);
                    return null;
                }
        }
    }
}
