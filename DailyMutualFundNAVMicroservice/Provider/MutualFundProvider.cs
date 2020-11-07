using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DailyMutualFundNAVMicroservice.Provider;
using DailyMutualFundNAVMicroservice.Models;
using DailyMutualFundNAVMicroservice.Repository;

namespace DailyMutualFundNAVMicroservice.Provider
{
    public class MutualFundProvider:IMutualFundProvider
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MutualFundProvider));
        private readonly IMutualFundRepository mutualFundRepository;
        public MutualFundProvider(IMutualFundRepository _mutualFundRepository)
        {
            mutualFundRepository = _mutualFundRepository;
        }
        public MutualFundDetails GetMutualFundByNameProvider(string mutualFundName)
        {
            MutualFundDetails mutualFundData = null;
            try
            {
                _log4net.Info("In MutualFundProvider, MutualFundNAVController has Called GetMutualFundByNameProvider and "+mutualFundName+" is searched");
                mutualFundData = mutualFundRepository.GetMutualFundByNameRepository(mutualFundName);
            }
            
            catch(Exception exception)
            {
                _log4net.Error("In MutualFundProvider, exception is found. "+exception.Message);
            }
            return mutualFundData;       
        }
    }
}