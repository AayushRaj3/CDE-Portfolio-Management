using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApi1.Models;

namespace AuthorizationApi1.Repository
{
    public interface IAuthenticationRepository
    {
        public string GenerateJSONWebToken(User user);
        public string Login(User login);
    }
}
