using DailySharePriceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DailySharePriceApi.Repository
{
    public class StockRepository : IStockRepository
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(StockRepository));

        public static List<Stock> stocklist = new List<Stock>()
        {
            new Stock { StockId = 101, StockName = "BTC", StockValue = 99.95},
            new Stock { StockId = 102, StockName = "ETH", StockValue = 40.2},
            new Stock {StockId = 103, StockName = "LTC", StockValue = 23.6}
        };
       
        /// <summary>
        /// It is a Method in Repository that is being called by the Provider and returns the Stock based on the StockName
        /// </summary>
        /// <param name="stockname"></param>
        /// <returns></returns>
        public Stock GetStockByNameRepository(string stockname)
        {
            Stock stockData = null;
            try
            {
                _log4net.Info("In Stock Repository, StockProvider is calling the Method GetStockByNameRepository and " + stockname + " is being searched");
                stockData = stocklist.FirstOrDefault(s => s.StockName == stockname);
                if (stockData != null)
                {
                    var jsonStock = JsonConvert.SerializeObject(stockData);
                    _log4net.Info("Stock Found " + jsonStock);
                }
                else
                {
                    _log4net.Info("In StockRepository, Stock " + stockname + " is not found");
                }
            }
            catch(Exception ex)
            {
                _log4net.Error("In Stock Repository, Exception Found - " + ex.Message);
            }
            return stockData;            
        }
    }
}
