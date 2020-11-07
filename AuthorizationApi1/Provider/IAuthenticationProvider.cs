using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationApi1.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthorizationApi1.Provider
{
    public interface IAuthenticationProvider
    {

        public string Login(User user);
    }
}
