using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateNetWorthApi.Models
{
    public class PortFolioDetails
    {
        public int PortFolioId { get; set; }

        public List<StockDetails> StockList { get; set; }

        public List<MutualFundDetails> MutualFundList { get; set; }
    }
}
