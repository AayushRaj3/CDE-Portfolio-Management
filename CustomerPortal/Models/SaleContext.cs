using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortal.Models
{
    public class SaleContext: DbContext
    {
        public SaleContext(DbContextOptions<SaleContext> options) : base(options)
        {

        }

        public DbSet<Sale> Sales { get; set; }
    }
}
