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
        /// <summary>
        /// This method adds the sale data to the database
        /// </summary>
        /// <param name="sale"></param>
        public void Add(Sale sale)
        {
            try
            {
                _log4net.Info("Entering the data in the database. The data is " + JsonConvert.SerializeObject(sale));
                _saleContext.Sales.Add(sale);
                _saleContext.SaveChanges();
            }
            catch(Exception ex)
            {
                _log4net.Error("An exception occured: "+ ex.Message);
            }

        }
    }
}
