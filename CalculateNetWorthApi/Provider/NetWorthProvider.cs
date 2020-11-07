using CalculateNetWorthApi.Models;
using CalculateNetWorthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateNetWorthApi.Provider
{
    public class NetWorthProvider : INetWorthProvider
    {
        private readonly INetWorthRepository _netWorthRepository;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NetWorthProvider));


        /// <summary>
        /// Constructor of Provider Class to initialise the network repositry object.
        /// </summary>
        /// <param name="netWorthRepository"></param>
        /// 
        public NetWorthProvider(INetWorthRepository netWorthRepository)
        {
            _netWorthRepository = netWorthRepository;
        }

        /// <summary>
        /// Calculating the networth of user with given portfolio details
        /// </summary>
        /// <param name="portFolioDetails"></param>
        /// <returns></returns>
        public Task<NetWorth> calculateNetWorthAsync(PortFolioDetails portFolioDetails)
        {
            NetWorth networth = new NetWorth();
            try
            {
                _log4net.Info("Provider called from Controller to calculate the networth");
                if (portFolioDetails.PortFolioId==0)
                {
                    return null;
                }
                networth = _netWorthRepository.calculateNetWorthAsync(portFolioDetails).Result;
            }
            catch(Exception ex)
            {
                _log4net.Error("Exception occured while calculating the networth"+ex.Message);
            }
            return Task.FromResult(networth);


        }

        /// <summary>
        /// Selling the assets of the user. The lsit contains two portfolios ofthe same person. In the first, we have his
        /// current portfolio and in the other, we have the portfolio he wants to sell.
        /// </summary>
        /// <param name="listOfAssetsCurrentlyHoldingAndAssetsToSell"></param>
        /// <returns></returns>

        public AssetSaleResponse sellAssets(List<PortFolioDetails> listOfAssetsCurrentlyHoldingAndAssetsToSell)
        {
            AssetSaleResponse assetSaleResponse = new AssetSaleResponse();
            try
            {

                
                if (listOfAssetsCurrentlyHoldingAndAssetsToSell.Any() == false)
                {
                    return null;
                }
                _log4net.Info(nameof(sellAssets) + " method called to sell assets of user with id = " + listOfAssetsCurrentlyHoldingAndAssetsToSell[0].PortFolioId);
                assetSaleResponse = _netWorthRepository.sellAssets(listOfAssetsCurrentlyHoldingAndAssetsToSell);
                
            }
            catch(Exception ex)
            {
                _log4net.Error("Exception occured while selling the assets:" + ex.Message);
            }
            return assetSaleResponse;
        }

        /// <summary>
        /// This method returns the portfolio of the user with the id provided in the parameter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PortFolioDetails GetPortFolioDetailsByID(int id)
        {
            PortFolioDetails portfolioDetails = new PortFolioDetails();
            try
            {
                _log4net.Info("Returning the portfolio object from the provider method of user :" + id);
                portfolioDetails = _netWorthRepository.GetPortFolioDetailsByID(id);
            }
            catch(Exception ex)
            {
                _log4net.Error("Exception ocured while getting the portfolio:"+ex.Message);
            }
            return portfolioDetails;
        }
    }
}
