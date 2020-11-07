using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyMutualFundNAVMicroservice.Models
{
    public class MutualFundDetails
    {
        public int MutualFundId { get; set; }
        public string MutualFundName { get; set; }
        public double MutualFundValue { get; set; }
    }
}
