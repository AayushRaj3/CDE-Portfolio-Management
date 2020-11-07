using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortal.Models
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        public int PortFolioID { get; set; }
        public double NetWorth { get; set; }

        public bool status { get; set; }
    }
}
