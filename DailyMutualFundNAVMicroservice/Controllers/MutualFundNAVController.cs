using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DailyMutualFundNAVMicroservice.Repository;
using DailyMutualFundNAVMicroservice.Models;
using DailyMutualFundNAVMicroservice.Provider;
using System.Net;

namespace DailyMutualFundNAVMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MutualFundNAVController : ControllerBase
    {
        static readonly log4net.ILog _log4net=log4net.LogManager.GetLogger(typeof(MutualFundNAVController));
        private readonly IMutualFundProvider mutualFundProvider;
        public MutualFundNAVController(IMutualFundProvider _mutualFundProvider)
        {
            _log4net.Info("MutualFundNAVController Initiated");
            mutualFundProvider = _mutualFundProvider;
        }
        [HttpGet("{mutualFundName}")]
        public IActionResult GetMutualFundDetailsByName(string mutualFundName)
        { 
            try
            {
                if (string.IsNullOrEmpty(mutualFundName))
                {
                    _log4net.Info("MutualFund Null Name.");
                    return BadRequest("Name Cannot be null");
                }
                _log4net.Info("In MutualFundNAV Controller, " + mutualFundName + " is searched");
                var mutualFundData = mutualFundProvider.GetMutualFundByNameProvider(mutualFundName);
                if (mutualFundData == null)
                {
                    _log4net.Info(mutualFundName+" is invalid MutualFund.");
                    return NotFound("Invalid MutualFund Name");
                }
                else
                {
                    _log4net.Info(mutualFundName+" MutualFund Found.");
                    return Ok(mutualFundData);
                }
                
            }
            catch (Exception exception)
            {
                _log4net.Error("Exception Found=" + exception.Message) ;   
                return new StatusCodeResult(500);
            }
        }
    }
}