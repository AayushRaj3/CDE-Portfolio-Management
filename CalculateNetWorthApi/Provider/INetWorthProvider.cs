using CalculateNetWorthApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateNetWorthApi.Provider
{
    public interface INetWorthProvider
    {
        public Task<NetWorth> calculateNetWorthAsync(PortFolioDetails portFolioDetails);

        public AssetSaleResponse sellAssets(List<PortFolioDetails> currentHoldingsAndToSell);

        public PortFolioDetails GetPortFolioDetailsByID(int id);
    }
}
