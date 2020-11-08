using CalculateNetWorthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;

namespace CalculateNetWorthApi.Repository
{ 
    public class NetworthRepository : INetWorthRepository
    {


        private IConfiguration configuration;


        /// <summary>
        /// Injecting IConfiguration for reading 
        /// </summary>
        /// <param name="_configuration"></param>
        public NetworthRepository(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NetworthRepository));

        public static List<PortFolioDetails> _portFolioDetails = new List<PortFolioDetails>()
            {
                new PortFolioDetails{
                    PortFolioId=12345,
                    MutualFundList = new List<MutualFundDetails>()
                    {
                        new MutualFundDetails{MutualFundName = "UDAAN", MutualFundUnits=44},
                        new MutualFundDetails{MutualFundName = "VIVA", MutualFundUnits=66}
                    },
                    StockList = new List<StockDetails>()
                    {
                        new StockDetails{StockCount = 19, StockName = "BTC"},
                        new StockDetails{StockCount = 667, StockName = "ETH"}
                    }
                },
                new PortFolioDetails
                {
                    PortFolioId = 789,
                    MutualFundList = new List<MutualFundDetails>()
                    {
                        new MutualFundDetails{MutualFundName = "UDAAN", MutualFundUnits=34},
                        new MutualFundDetails{MutualFundName = "VIVA", MutualFundUnits=566}
                    },
                    StockList = new List<StockDetails>()
                    {
                        new StockDetails{StockCount = 240, StockName = "BTC"},
                        new StockDetails{StockCount = 46, StockName = "LTC"}
                    }
                },
                new PortFolioDetails
                {
                    PortFolioId = 1729,
                    MutualFundList = new List<MutualFundDetails>()
                    {
                        new MutualFundDetails{MutualFundName = "CRED", MutualFundUnits=8},
                        new MutualFundDetails{MutualFundName = "UDAAN", MutualFundUnits=6},
                        new MutualFundDetails{MutualFundName = "VIVA", MutualFundUnits=6}
                    },
                    StockList = new List<StockDetails>()
                    {
                        new StockDetails{StockCount = 20, StockName = "LTC"},
                        new StockDetails{StockCount = 34, StockName = "ETH"},
                        new StockDetails{StockCount = 12, StockName = "BTC"}
                    }
                }

            };

        /// <summary>
        /// This Will calculate the networth of the Client based on the number of stocks and mutual Funds he has.
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        /// 

        public async Task<NetWorth> calculateNetWorthAsync(PortFolioDetails portFolioDetails)
        {
            
            Stock stock = new Stock();
            MutualFund mutualfund = new MutualFund();
            NetWorth networth = new NetWorth();
            _log4net.Info("Calculating the networth in the repository method of user with id = "+portFolioDetails.PortFolioId);
            try
            {
                using (var httpClient = new HttpClient())
                {

                    var fetchStock = configuration["GetStockDetails"];
                    var fetchMutualFund = configuration["GetMutualFundDetails"];
                    if (portFolioDetails.StockList != null && portFolioDetails.StockList.Any() == true)
                    {
                        foreach (StockDetails stockDetails in portFolioDetails.StockList)
                        {
                            if (stockDetails.StockName != null)
                            {
                                using (var response = await httpClient.GetAsync(fetchStock + stockDetails.StockName))
                                {
                                    _log4net.Info("Fetching the details of stock "+stockDetails.StockName+"from the stock api");
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    stock = JsonConvert.DeserializeObject<Stock>(apiResponse);
                                }
                                networth.Networth += stockDetails.StockCount * stock.StockValue;
                            }
                        }
                    }
                    if (portFolioDetails.MutualFundList != null && portFolioDetails.MutualFundList.Any() == true)
                    {
                        foreach (MutualFundDetails mutualFundDetails in portFolioDetails.MutualFundList)
                        {
                            if (mutualFundDetails.MutualFundName != null)
                            {
                                using (var response = await httpClient.GetAsync(fetchMutualFund + mutualFundDetails.MutualFundName))
                                {
                                    _log4net.Info("Fetching the details of mutual Fund " + mutualFundDetails.MutualFundName + "from the MutualFundNAV api");
                                    string apiResponse = await response.Content.ReadAsStringAsync();
                                    mutualfund = JsonConvert.DeserializeObject<MutualFund>(apiResponse);
                                }
                                networth.Networth += mutualFundDetails.MutualFundUnits * mutualfund.MutualFundValue;
                            }
                        }
                    }
                }
                networth.Networth = Math.Round(networth.Networth, 2);
            }
            catch(Exception ex)
            {
                _log4net.Error("Exception occured while calculating the networth of user"+portFolioDetails.PortFolioId+":"+ex.Message);
            }
            return networth;
        }

        /// <summary>
        /// The list contains two PortFolio objects. The first one has the current holdings, the other has the list of stocks or mutual funds 
        /// the user wants to sell. The function returns whether the sale is successful and also returns the new networth.
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public AssetSaleResponse sellAssets(List<PortFolioDetails> details)
        {
            NetWorth networth = new NetWorth();
            NetWorth networth2 = new NetWorth();

            int flag1 = 1;
            int flag2 = 1;
            StockDetails stocktoremove = new StockDetails();
            MutualFundDetails fundToRemove = new MutualFundDetails();

            AssetSaleResponse assetSaleResponse = new AssetSaleResponse();
            PortFolioDetails current = details[0];
            PortFolioDetails toSell = details[1];
            try
            {
                
                foreach (PortFolioDetails portFolioDetails in details)
                {
                    if (portFolioDetails == null)
                    {
                        return null;
                    }
                }

                _log4net.Info("Selling the assets of user with id" + details[0].PortFolioId);
                networth = calculateNetWorth(current).Result;
                assetSaleResponse.SaleStatus = true;


                foreach (StockDetails toSellStock in toSell.StockList)
                {
                    foreach (StockDetails currentStock in current.StockList)
                    {
                        if (currentStock.StockName == toSellStock.StockName)
                        {
                            if (currentStock.StockCount < toSellStock.StockCount)
                            {
                                _log4net.Info("Not enough stocks to sell for user :"+current.PortFolioId);
                                assetSaleResponse.SaleStatus = false;
                                assetSaleResponse.Networth = networth.Networth;
                                return assetSaleResponse;
                            }
                            break;
                        }
                        
                    }
                }


                foreach (MutualFundDetails toSellMutualFund in toSell.MutualFundList)
                {
                    foreach (MutualFundDetails currentMutualFund in current.MutualFundList)
                    {
                        if (currentMutualFund.MutualFundName == toSellMutualFund.MutualFundName)
                        {
                            if (currentMutualFund.MutualFundUnits < toSellMutualFund.MutualFundUnits)
                            {
                                _log4net.Info("Not enough mutualFunds to sell for user"+current.PortFolioId);
                                assetSaleResponse.SaleStatus = false;
                                assetSaleResponse.Networth = networth.Networth;
                                return assetSaleResponse;
                            }
                            break;
                        }
                        
                    }
                }

                


                foreach (PortFolioDetails portfolio in _portFolioDetails)
                {
                    if (portfolio.PortFolioId == toSell.PortFolioId)
                    {
                        foreach (StockDetails currentstock in portfolio.StockList)
                        {
                            foreach (StockDetails sellstock in toSell.StockList)
                            {
                                if (sellstock.StockName == currentstock.StockName)
                                {
                                    currentstock.StockCount = currentstock.StockCount - sellstock.StockCount;
                                    if (currentstock.StockCount == 0)
                                    {
                                        _log4net.Info("The user with id = "+ current.PortFolioId+ "sold all of his "+currentstock.StockName+" stocks.");
                                        flag1 = 0;
                                        stocktoremove = currentstock;
                                        
                                        break;
                                    }
                                    
                                }
       
                            }

                        }


                        foreach (MutualFundDetails currentmutualfund in portfolio.MutualFundList)
                        {
                            foreach (MutualFundDetails sellmutualfund in toSell.MutualFundList)
                            {
                                if (sellmutualfund.MutualFundName == currentmutualfund.MutualFundName)
                                {
                                    currentmutualfund.MutualFundUnits = currentmutualfund.MutualFundUnits - sellmutualfund.MutualFundUnits;
                                    if (currentmutualfund.MutualFundUnits == 0)
                                    {
                                        _log4net.Info("The user with id = " + current.PortFolioId + " has sold all of his mutual funds of" + currentmutualfund.MutualFundName);
                                        flag2 = 0;
                                        fundToRemove = currentmutualfund;
                                        
                                        break;
                                    }
                                    
                                }
                                
                            }
                        }
                    }
                }
                if (flag1 == 0)
                {
                    foreach (PortFolioDetails portfolio in _portFolioDetails)
                    {
                        if (portfolio.PortFolioId == current.PortFolioId)
                        {
                            portfolio.StockList.Remove(stocktoremove);
                        }
                    }
                }
                if (flag2 == 0)
                {
                    foreach (PortFolioDetails portfolio in _portFolioDetails)
                    {
                        if (portfolio.PortFolioId == current.PortFolioId)
                        {
                            portfolio.MutualFundList.Remove(fundToRemove);
                        }
                    }
                }
                
                networth2 = calculateNetWorth(toSell).Result;
                assetSaleResponse.Networth = networth.Networth - networth2.Networth;
                //
            }
            catch(Exception ex)
            {
                _log4net.Error("An exception occured while selling the assets:" + ex.Message);
            }
            return assetSaleResponse;
        }

        /// <summary>
        /// Calculates the networth based on the details provided in the portfolio object
        /// </summary>
        /// <param name="portFolio"></param>
        /// <returns></returns>
        public async Task<NetWorth> calculateNetWorth(PortFolioDetails portFolio)
        {
            try
            {
                NetWorth _networth = new NetWorth();

                Stock stock = new Stock();
                MutualFund mutualFund = new MutualFund();
                double networth = 0;

                using (var httpClient = new HttpClient())
                {

                    var fetchStock = configuration["GetStockDetails"];
                    var fetchMutualFund = configuration["GetMutualFundDetails"];
                    if (portFolio.StockList != null || portFolio.MutualFundList.Any() != false)
                    {
                        foreach (StockDetails stockDetails in portFolio.StockList)
                        {
                            using (var response = await httpClient.GetAsync(fetchStock + stockDetails.StockName))
                            {
                                _log4net.Info("Fetching the details of stock "+ stockDetails.StockName+" from the stock api");
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                stock = JsonConvert.DeserializeObject<Stock>(apiResponse);
                            }
                            networth += stockDetails.StockCount * stock.StockValue;
                        }
                    }
                    if (portFolio.MutualFundList != null || portFolio.MutualFundList.Any() != false)
                    {
                        foreach (MutualFundDetails mutualFundDetails in portFolio.MutualFundList)
                        {
                            using (var response = await httpClient.GetAsync(fetchMutualFund + mutualFundDetails.MutualFundName))
                            {
                                _log4net.Info("Fetching the details of stock " + mutualFundDetails.MutualFundName + " from the stock api");
                                string apiResponse = await response.Content.ReadAsStringAsync();
                                mutualFund = JsonConvert.DeserializeObject<MutualFund>(apiResponse);
                            }
                            networth += mutualFundDetails.MutualFundUnits * mutualFund.MutualFundValue;
                        }
                    }
                }
                networth = Math.Round(networth, 2);
                _networth.Networth = networth;
                return _networth;

            }
            catch (Exception ex)
            {
                _log4net.Error("An exception occured while selling the assets:" + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Returns the PortFolio object with the wanted id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public PortFolioDetails GetPortFolioDetailsByID(int id)
        {
            PortFolioDetails portFolioDetails = new PortFolioDetails();
            try
            {
                _log4net.Info("Returning portfolio details with the id" + id + " from the repository method");
                portFolioDetails = _portFolioDetails.FirstOrDefault(e => e.PortFolioId == id);
            }
            catch(Exception ex)
            {
                _log4net.Error("An exception occured while fetching the portFolio details:"+ex.Message);
                return null;
            }
            return portFolioDetails;
        }
    }
}
