using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateNetWorthApi.Models
{
    public class MutualFund
    {
        public int MutualFundId { get; set; }
        public string MutualFundName { get; set; }
        public double MutualFundValue { get; set; }
    }
}
