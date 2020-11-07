using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortal.Models.ViewModel
{
    public class SellingViewModel
    {
        
        public int PortFolioID { get; set; }

        public StockDetails sd { get; set; }

        public MutualFundDetails md { get; set; }

    }
}
