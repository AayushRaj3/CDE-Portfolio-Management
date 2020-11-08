using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortal.Models.ViewModel
{
    public class CompleteDetails
    {
        public int PFId { get; set; }

        public List<CompleteStockDetails> FinalStockList { get; set; }

        public List<CompleteMutualFundDetails> FinalMutualFundList { get; set; }

        public double NetWorth { get; set; }
    }
}
