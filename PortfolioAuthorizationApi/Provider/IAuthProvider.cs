using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioAuthorizationApi.Models;

namespace PortfolioAuthorizationApi.Provider
{
    public interface IAuthProvider
    {
        public string AuthenticateUser(User user);
    }
}
