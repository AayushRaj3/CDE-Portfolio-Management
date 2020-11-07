using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalculateNetWorthApi.Models;
using CalculateNetWorthApi.Provider;
using CalculateNetWorthApi.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CalculateNetWorthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetWorthController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NetWorthController));

        private readonly INetWorthProvider _netWorthProvider;

        public NetWorthController(INetWorthProvider netWorthProvider)
        {
            _netWorthProvider = netWorthProvider;
        }

        /// <summary>
        /// Returns the portfolio of the user with id equal to the one in parameter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        

        [HttpGet("{portFolioId}")]
        public IActionResult GetPortFolioDetailsByID(int portFolioId)
        {
            PortFolioDetails portFolioDetails = new PortFolioDetails();
            try
            {
                if (portFolioId <=0)
                {
                    return NotFound("ID can't be 0 or less than 0");
                }
                portFolioDetails = _netWorthProvider.GetPortFolioDetailsByID(portFolioId);
                if (portFolioDetails == null)
                {
                    return NotFound("Sorry, We don't have a portfolio with that ID");
                }
                else
                {
                    _log4net.Info("The deatils of the user are" +portFolioDetails);
                    return Ok(portFolioDetails);
                }
            }
            catch(Exception ex)
            {
                _log4net.Info(ex.Message);
                return new StatusCodeResult(500);
            }  
        }

        /// <summary>
        /// Returns the networth based on the portfolio details sent
        /// </summary>
        /// <param name="portFolioDetails"></param>
        /// <returns></returns>

        // GET api/<NetWorthController>/5
        [HttpPost]
        public IActionResult GetNetWorth(PortFolioDetails portFolioDetails)
        {

            NetWorth _netWorth = new NetWorth();
            _log4net.Info("Calculating the networth of user with id = "+ portFolioDetails.PortFolioId);

            try
            {
                if (portFolioDetails == null)
                {
                    return NotFound("The portfolio doesn't contain any data");
                }
                else if (portFolioDetails.PortFolioId == 0)
                {
                    return NotFound("The user with that id not found");
                }
                else if(portFolioDetails.StockList.Any() == false && portFolioDetails.MutualFundList.Any())
                {
                    return NotFound("The user doesn't hold any assets");
                }
                else if ((portFolioDetails.MutualFundList == null || portFolioDetails.MutualFundList.Any()==false) && (portFolioDetails.StockList == null || portFolioDetails.StockList.Any()==false))
                {
                    _log4net.Info("The user does not hold any assets");
                    return NotFound("Sorry, the user doesn't hold any assets");
                }
                else
                {
                    _log4net.Info("The portfolio details are correct.Returning the networth of user with id"+portFolioDetails.PortFolioId);
                    _netWorth = _netWorthProvider.calculateNetWorthAsync(portFolioDetails).Result;
                    _log4net.Info("The networth is:"+_netWorth);
                    return Ok(_netWorth);
                }     
            }
            catch (Exception ex)
            {
                _log4net.Info("An exception occured:"+ex);
                return new StatusCodeResult(500);
            }
        }


        /// <summary>
        /// The list contains two portfolios. One is the current holdings of the user and the other is the ones he wants to sell.
        ///The method returns the new networth after subtracting the assets he sold. IT also returns the sale status whether the sale was 
        ///successful or not.
        /// </summary>
        /// <param name="listOfAssetsCurrentlyHoldingAndAssetsToSell"></param>
        /// <returns></returns>
        // POST api/<NetWorthController>
        [HttpPost]
        [Route("SellAssets")]
        public IActionResult SellAssets(List<PortFolioDetails> listOfAssetsCurrentlyHoldingAndAssetsToSell)
        {
            try
            {
                AssetSaleResponse assetSaleResponse = new AssetSaleResponse();
                if (listOfAssetsCurrentlyHoldingAndAssetsToSell == null || listOfAssetsCurrentlyHoldingAndAssetsToSell.Any()==false)
                {
                    _log4net.Info("The portfolio provided don't hold any data");
                    return NotFound("Please Provide a Valid List of portFolios");
                }
                else
                {

                    foreach(PortFolioDetails portFolioDetails in listOfAssetsCurrentlyHoldingAndAssetsToSell)
                    {
                        if(portFolioDetails==null || portFolioDetails.PortFolioId == 0)
                        {
                            return NotFound("We didn't find any portfolio with that Id");
                        }
                    }
                    assetSaleResponse = _netWorthProvider.sellAssets(listOfAssetsCurrentlyHoldingAndAssetsToSell);
                    if (assetSaleResponse == null)
                    {
                        _log4net.Info("Couldn't be sold because of invalid portfolio");
                        return NotFound("Please provide a valid list of portfolios");
                    }
                    return Ok(assetSaleResponse);
                }
            }
            catch(Exception ex)
            {
                _log4net.Info(ex);
                return NotFound(ex.Message);
            }
        }
    }
}
