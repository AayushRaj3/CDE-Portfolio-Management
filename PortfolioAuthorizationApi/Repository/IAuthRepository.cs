using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioAuthorizationApi.Models;

namespace PortfolioAuthorizationApi.Repository
{
    public interface IAuthRepository
    {
        public string GenerateToken(User user);
    }
}
