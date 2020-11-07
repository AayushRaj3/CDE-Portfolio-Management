using CalculateNetWorthApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateNetWorthApi.Repository
{
    public interface INetWorthRepository
    {
        public Task<NetWorth> calculateNetWorthAsync(PortFolioDetails portFolioDetails);

        public AssetSaleResponse sellAssets(List<PortFolioDetails> currentHoldingAndToSell);

        public PortFolioDetails GetPortFolioDetailsByID(int id);
    }
}
