using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DailyMutualFundNAVMicroservice.Models;
using DailyMutualFundNAVMicroservice.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DailyMutualFundNAVMicroservice.Repository
{
    public class MutualFundRepository:IMutualFundRepository
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MutualFundRepository));
        private static List<MutualFundDetails> listOfFunds = new List<MutualFundDetails>()
        { 
            new MutualFundDetails { MutualFundId = 1, MutualFundName = "UDAAN", MutualFundValue = 135.84},
            new MutualFundDetails { MutualFundId = 2, MutualFundName = "VIVA", MutualFundValue = 100.14},
            new MutualFundDetails { MutualFundId = 2, MutualFundName = "CRED", MutualFundValue = 104.14}
        };
          /// <summary>
          ///  This will return the MutualFund Details by name
          /// </summary>
          /// <param name="mutualFundName"></param>
          /// <returns></returns>
          public MutualFundDetails GetMutualFundByNameRepository(string mutualFundName)
          {
            MutualFundDetails mutualFundData = null;
            try
            {
                string mutualfundName = mutualFundName.ToUpper();
                _log4net.Info("In MutualFundRepository, MutualFundProvider has Called GetMutualFundByNameRepository and " + mutualFundName + " is searched");
                 mutualFundData = listOfFunds.Where(e => e.MutualFundName == mutualfundName).FirstOrDefault();    
                if(mutualFundData!=null)
                {
                    var jsonFund = JsonConvert.SerializeObject(mutualFundData);
                    _log4net.Info("Mutual Fund Found "+jsonFund);
                }
                else
                {
                    _log4net.Info("In MutualFundRepository, MutualFund "+mutualFundName+" is not found.");
                }
            }
            catch(Exception exception)
            {
                _log4net.Error("Exception occurred while finding the mutual fund details "+exception.Message);
            }
            return mutualFundData;
          }
    }
}