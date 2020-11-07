using CustomerPortal.Models;
using CustomerPortal.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerPortal.Repository
{
    public interface ISaleRepository
    {
        public void Add(Sale sale);

    }
}
