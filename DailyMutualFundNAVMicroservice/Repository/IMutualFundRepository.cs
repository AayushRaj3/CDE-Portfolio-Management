using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyMutualFundNAVMicroservice.Models;

namespace DailyMutualFundNAVMicroservice.Repository
{
    public interface IMutualFundRepository
    {
        public MutualFundDetails GetMutualFundByNameRepository(string mutualFundName);
    }
}
