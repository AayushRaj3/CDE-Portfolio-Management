using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CustomerPortal.Models;
using CustomerPortal.Models.ViewModel;
using CustomerPortal.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomerPortal.Controllers
{
    public class PortFolioController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PortFolioController));
        private ISaleRepository _saleRepository;
        private IConfiguration configuration;
        
        public PortFolioController(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }


        /// <summary>
        /// This method logs out the user and clears the session
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            try
            {
                string portfolioid = HttpContext.Session.GetString("Id");
                _log4net.Info(" User with Id = " + portfolioid + " is logging out");
                HttpContext.Session.Clear();
            }
            catch(Exception ex)
            {
                _log4net.Error("An exception occured while logging out the employee:"+ex.Message);
                return new StatusCodeResult(500);
            }
            return RedirectToAction("Login", "Home");
        } 


        /// <summary>
        /// This method checks whether the session token is null or not. Returns true if not, meaning that user is validated
        /// and returns false if it is nul, meaning the user is not validated and won't be procedded further
        /// </summary>
        /// <returns></returns>
        public bool CheckValid()
        {
            try
            {
                if (HttpContext.Session.GetString("JWTtoken") != null)
                {
                    _log4net.Info(" User with Id = " + HttpContext.Session.GetString("Id") + " is a valid user");
                    return true;
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("Exception occured whille checking the validity of th token of user with id = " + HttpContext.Session.GetString("Id")+"The exception is:"+ex.Message);
            }
            return false;
        }


        /// <summary>
        /// This method shows the portfolio details of the user.
        /// </summary>
        /// <returns></returns>
        // GET: PortFolioController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                NetWorth _netWorth = new NetWorth();
                if (CheckValid())
                {
                    int portfolioid = Convert.ToInt32(HttpContext.Session.GetString("Id"));
                    //CompleteDetails completeDetails = _saleRepository.CalculateNetWorth(portfolioid).Result;
                    _log4net.Info("Showing the user with portfolio ID = " + portfolioid + "his networth and the assets he is currently holding");
                    CompleteDetails completeDetails = new CompleteDetails();
                    PortFolioDetails portFolioDetails = new PortFolioDetails();


                    using (var client = new HttpClient())
                    {
                        using (var response = await client.GetAsync("https://localhost:44375/api/NetWorth/" + portfolioid))
                        {
                            _log4net.Info("Calling the Calculate Networth Api");
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            portFolioDetails = JsonConvert.DeserializeObject<PortFolioDetails>(apiResponse);
                        }
                    }
                    StringContent content = new StringContent(JsonConvert.SerializeObject(portFolioDetails), Encoding.UTF8, "application/json");
                    using (var client = new HttpClient())
                    {
                        using (var response = await client.PostAsync("https://localhost:44375/api/NetWorth", content))
                        {
                            _log4net.Info("Calling the Networth api to return the networth of sent portfolio");
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            _netWorth = JsonConvert.DeserializeObject<NetWorth>(apiResponse);

                        }
                    }


                    completeDetails.PFId = portFolioDetails.PortFolioId;
                    completeDetails.FinalMutualFundList = portFolioDetails.MutualFundList;
                    completeDetails.FinalStockList = portFolioDetails.StockList;

                    completeDetails.NetWorth = _netWorth.networth;
                    return View(completeDetails);
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("An exception occured while showing the portfolio details to the user. the message is :"+ex.Message);
            }
            return RedirectToAction("Index", "Home");
        }


        /// <summary>
        /// this method is invoked when user wants to sell a stock. It then returns a view asking them for how many
        /// stocks they want to sell.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult SellStock(string name)
        {
            StockDetails stockdetail = new StockDetails();
            stockdetail.StockName = name;
            return View(stockdetail);

        }

        /// <summary>
        /// this method is invoked when user wants to sell a mutual Fund. It then returns a view asking them for how many
        /// mutual fund they want to Sell.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult SellMutualFund(string name)
        {
            MutualFundDetails mutualdetail = new MutualFundDetails();
            mutualdetail.MutualFundName = name;
            return View(mutualdetail);
        }



        [HttpPost]
        public async Task<IActionResult> SellStock(StockDetails stockdetails)
        {
            PortFolioDetails current = new PortFolioDetails();
            PortFolioDetails toSell = new PortFolioDetails();
            int id = Convert.ToInt32(HttpContext.Session.GetString("Id"));
            _log4net.Info("Selling the stocks of user with id = " +id);
            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://localhost:44375/api/NetWorth/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    current = JsonConvert.DeserializeObject<PortFolioDetails>(apiResponse);
                }
            }
            toSell.PortFolioId = id;
            toSell.StockList = new List<StockDetails>
            {
                stockdetails
            };
            toSell.MutualFundList = new List<MutualFundDetails>() { };

            List<PortFolioDetails> list = new List<PortFolioDetails>
            {
                current,
                toSell
            };

            AssetSaleResponse assetSaleResponse = new AssetSaleResponse();
            StringContent content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                using (var response = await client.PostAsync("https://localhost:44375/api/NetWorth/SellAssets", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    assetSaleResponse = JsonConvert.DeserializeObject<AssetSaleResponse>(apiResponse);
                }
            }
            _log4net.Info("sale of stock of user with id" +current.PortFolioId+ "done");
            Sale _sale = new Sale();
            _sale.PortFolioID = current.PortFolioId;
            _sale.NetWorth = assetSaleResponse.Networth;
            _sale.status = assetSaleResponse.SaleStatus;
            _saleRepository.Add(_sale);
            
            return View("Reciept", assetSaleResponse);
        }

        [HttpPost]
        public async Task<IActionResult> SellMutualFund(MutualFundDetails mutualFundDetails)
        {
            PortFolioDetails current = new PortFolioDetails();
            PortFolioDetails toSell = new PortFolioDetails();
            int id = Convert.ToInt32(HttpContext.Session.GetString("Id"));

            using (var client = new HttpClient())
            {
                using (var response = await client.GetAsync("https://localhost:44375/api/NetWorth/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    current = JsonConvert.DeserializeObject<PortFolioDetails>(apiResponse);
                }
            }
            toSell.PortFolioId = id;
            toSell.MutualFundList = new List<MutualFundDetails>
            {
                mutualFundDetails
            };
            toSell.StockList = new List<StockDetails>();

            List<PortFolioDetails> list = new List<PortFolioDetails>
            {
                current,
                toSell
            };

            AssetSaleResponse assetSaleResponse = new AssetSaleResponse();
            StringContent content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8, "application/json");
            using (var client = new HttpClient())
            {
                using (var response = await client.PostAsync("https://localhost:44375/api/NetWorth/SellAssets", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    assetSaleResponse = JsonConvert.DeserializeObject<AssetSaleResponse>(apiResponse);
                }
            }
            _log4net.Info("sale of  mutual fund of user with id" + current.PortFolioId + "done");
            Sale _sale = new Sale();
            _sale.PortFolioID = current.PortFolioId;
            _sale.NetWorth = assetSaleResponse.Networth;
            _sale.status = assetSaleResponse.SaleStatus;
            _saleRepository.Add(_sale);

            return View("Reciept", assetSaleResponse);
        }

    }
}
