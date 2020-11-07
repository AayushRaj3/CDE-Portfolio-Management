using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomerPortal.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CustomerPortal.Controllers
{
    public class HomeController : Controller
    {

        private IConfiguration _configuration;
        //private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration configuration)
        {
            //_logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(Users user)
        {
            //string token = GetToken("https://localhost:44354/api/Auth", user);
            var connection = _configuration["AuthenticationToken"];
            string token = GetToken(connection, user);
            /*if (token != null)
            {
                return RedirectToAction("Index", "Meds", new { name = token });
            }*/

            if (token != null)
            {
                HttpContext.Session.SetString("JWTtoken", token);
                HttpContext.Session.SetString("Id", user.PortFolioID.ToString());
                //ViewBag.Login = user.Username;
                return RedirectToAction("Index", "PortFolio");
            }
            else
            {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Username or Password is Incorrect");
                ViewBag.invalid = "UserId or Password invalid";
                return View();
            }
        }
        static string GetToken(string url, Users user)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url, data).Result;
                string name = response.Content.ReadAsStringAsync().Result;
                dynamic details = JObject.Parse(name);
                return details.tokenString;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
