using CalculateNetWorthApi.Controllers;
using CalculateNetWorthApi.Models;
using CalculateNetWorthApi.Provider;
using CalculateNetWorthApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateNetWorthUnitTesting
{
    public class CalculateNetWorthApiTests
    {
        public List<PortFolioDetails> _portFolioDetails;
        public NetWorthProvider netWorthProvider;
        private readonly Mock<INetWorthRepository> netWorthRepository = new Mock<INetWorthRepository>();
        private readonly Mock<INetWorthProvider> networthProviderMock = new Mock<INetWorthProvider>();
        public double networth;
        NetWorth netWorth;
        public NetWorthController networthController;
        AssetSaleResponse assetSaleResponse;

        public CalculateNetWorthApiTests()
        {
            netWorthProvider = new NetWorthProvider(netWorthRepository.Object);
            networthController = new NetWorthController(networthProviderMock.Object);
        }

        [SetUp]
        public void Setup()
        {

            networth = 10789.97;
            netWorth = new NetWorth();
            netWorth.Networth = networth;
            assetSaleResponse = new AssetSaleResponse() { SaleStatus = true, Networth = 12456.44 };
            _portFolioDetails = new List<PortFolioDetails>(){
                new PortFolioDetails
                {
                    PortFolioId = 123,
                    MutualFundList = new List<MutualFundDetails>()
                    {
                        new MutualFundDetails{MutualFundName = "Cred", MutualFundUnits=3},
                        new MutualFundDetails{MutualFundName = "Viva", MutualFundUnits=5}
                    },
                    StockList = new List<StockDetails>()
                    {
                        new StockDetails{StockCount = 1, StockName = "BTC"},
                        new StockDetails{StockCount = 6, StockName = "ETH"}
                    }
                },
                new PortFolioDetails
                {
                    PortFolioId = 12345,
                    MutualFundList = new List<MutualFundDetails>()
                    {
                        new MutualFundDetails{MutualFundName = "Cred", MutualFundUnits=1},
                        new MutualFundDetails{MutualFundName = "Viva", MutualFundUnits=1}
                    },
                    StockList = new List<StockDetails>()
                    {
                        new StockDetails{StockCount = 1, StockName = "BTC"},
                        new StockDetails{StockCount = 2, StockName = "ETH"}
                    }
                }
            };
        }


        //Testing for Repository starts here
        [Test]
        public void TestForCalculatingNetWorthWhenObjectHasValuesinRepository()
        {
            netWorthRepository.Setup(x => x.calculateNetWorthAsync(_portFolioDetails[0])).ReturnsAsync(netWorth);
            var res = netWorthProvider.calculateNetWorthAsync(_portFolioDetails[0]).Result;
            Assert.AreEqual(res.Networth, networth);
            Assert.Pass();
        }

        [Test]
        public void TestForCalculatingNetWorthWhenObjectisNullinRepository()
        {
            netWorthRepository.Setup(x => x.calculateNetWorthAsync(It.Ref<PortFolioDetails>.IsAny)).ReturnsAsync(()=>null);
            PortFolioDetails portfolio = new PortFolioDetails();
            var result = netWorthProvider.calculateNetWorthAsync(portfolio);
            Assert.IsNull(result);
            Assert.Pass();
        }

        [Test]
        public void TestForResponseWhenObjectHaveValuesinRepository()
        {
            netWorthRepository.Setup(x => x.sellAssets(_portFolioDetails)).Returns(assetSaleResponse);
            var result = netWorthProvider.sellAssets(_portFolioDetails);
            Assert.AreEqual(result.Networth, assetSaleResponse.Networth);
        }

        [Test]
        public void TestForResponseWhenEitherObjectIsNullinRepository()
        {
            netWorthRepository.Setup(x => x.sellAssets(It.Ref<List<PortFolioDetails>>.IsAny)).Returns(() => null);
            List<PortFolioDetails> portfoliodetails = new List<PortFolioDetails>();
            var result = netWorthProvider.sellAssets(portfoliodetails);
            Assert.IsNull(result);

        }

        [Test]
        public void TestForPortFolioObjectWhenWePassValidValueinRepository()
        {
            netWorthRepository.Setup(x => x.GetPortFolioDetailsByID(
                It.IsAny<int>())).Returns((int id) => _portFolioDetails.FirstOrDefault( ex => ex.PortFolioId==id));
            var result = netWorthProvider.GetPortFolioDetailsByID(123);
            Assert.IsNotNull(result);

        }

        [Test]
        public void TestForPortFolioObjectWhenWePassInvalidValueinRepository()
        {
            netWorthRepository.Setup(x => x.GetPortFolioDetailsByID(It.IsAny<int>())).Returns((int id) => _portFolioDetails.FirstOrDefault(ex => ex.PortFolioId == id));
            var result = netWorthProvider.GetPortFolioDetailsByID(1);
            Assert.IsNull(result);

        }


        //testing for provider starts here.
        [Test]
        public void TestForCalculatingNetWorthWhenObjectHasValuesinProvider()
        {
            networthProviderMock.Setup(x => x.calculateNetWorthAsync(_portFolioDetails[0])).ReturnsAsync(new NetWorth { Networth = networth });
            var data = networthController.GetNetWorth(_portFolioDetails[0]);
            ObjectResult okResult = data as ObjectResult;
            
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.Pass();

        }

        [Test]
        public void TestForCalculatingNetWorthWhenObjectDoesNotHaveValuesinProvider()
        {
            PortFolioDetails portFolioDetails = new PortFolioDetails();
            networthProviderMock.Setup(x => x.calculateNetWorthAsync(It.Ref<PortFolioDetails>.IsAny)).ReturnsAsync(() => null);
            var data = networthController.GetNetWorth(portFolioDetails);
            ObjectResult okResult = data as ObjectResult;

            Assert.AreEqual(404, okResult.StatusCode);
            Assert.Pass();

        }

        [Test]

        public void TestForResponseWhenObjectHaveValuesinProvider()
        {
            networthProviderMock.Setup(x => x.sellAssets(_portFolioDetails)).Returns(assetSaleResponse);
            var result = networthController.SellAssets(_portFolioDetails);
            ObjectResult okResult = result as ObjectResult;
            Assert.AreEqual(okResult.StatusCode, 200);
        }

        [Test]
        public void TestForResponseWhenObjectDoesNotHaveValuesinProvider()
        {
            networthProviderMock.Setup(x => x.sellAssets(It.Ref<List<PortFolioDetails>>.IsAny)).Returns(()=>null);
            var result = networthController.SellAssets(_portFolioDetails);
            ObjectResult okResult = result as ObjectResult;
            Assert.AreEqual(okResult.StatusCode, 404);
        }

        [Test]
        public void TestForPortFolioObjectWhenWePassValueinProvider()
        {
            networthProviderMock.Setup(
                x => x.GetPortFolioDetailsByID(It.IsAny<int>())).Returns(
                (int s) => _portFolioDetails.FirstOrDefault(e => e.PortFolioId == s));
            var data = networthController.GetPortFolioDetailsByID(12345);
            ObjectResult Okresult = data as ObjectResult;
            Assert.AreEqual(200, Okresult.StatusCode);
        }

        [Test]
        public void TestForPortFolioObjectWhenWePassInvalidValueinProvider()
        {
            networthProviderMock.Setup(
                x => x.GetPortFolioDetailsByID(It.IsAny<int>())).Returns(
                (int s) => _portFolioDetails.FirstOrDefault(e => e.PortFolioId == s));
            var data = networthController.GetPortFolioDetailsByID(1);
            ObjectResult Okresult = data as ObjectResult;
            Assert.AreEqual(404, Okresult.StatusCode);
        }














    }
}