using CustomerPortal.Models;
using CustomerPortal.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerPortal.Repository
{
    public class SaleRepository:ISaleRepository
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SaleRepository));
        private SaleContext _saleContext;
        public SaleRepository(SaleContext saleContext)
        {
            _saleContext = saleContext;
        }
        public void Add(Sale sale)
        {
            _saleContext.Sales.Add(sale);
            _saleContext.SaveChanges();

        }
    }
}
