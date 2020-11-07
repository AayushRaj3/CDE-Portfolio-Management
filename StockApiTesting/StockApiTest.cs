using Castle.Core.Configuration;
using DailySharePriceApi.Controllers;
using DailySharePriceApi.Models;
using DailySharePriceApi.Provider;
using DailySharePriceApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace StockApiTesting
{
    public class Tests
    {
        List<Stock> stocks = new List<Stock>();
        readonly StockController stockController;
        readonly StockProvider stockProvider;
        private readonly Mock<IStockProvider> mockProvider = new Mock<IStockProvider>();
        private readonly Mock<IStockRepository> mockRepository = new Mock<IStockRepository>();

        public Tests()
        {
            stockController = new StockController(mockProvider.Object);
            stockProvider = new StockProvider(mockRepository.Object);
        }
        

        [SetUp]
        public void Setup()
        {
            stocks = new List<Stock>()
            {
                new Stock{ StockId=1, StockName="DUMMY", StockValue=1},
                new Stock{ StockId=2, StockName="APP", StockValue=2}
            };

            mockProvider.Setup(x => x.GetStockByNameProvider(It.IsAny<string>())).Returns((string s) => stocks.FirstOrDefault(
                x => x.StockName.Equals(s)));

            mockRepository.Setup(x => x.GetStockByNameRepository(It.IsAny<string>())).Returns((string s) => stocks.FirstOrDefault(
                x => x.StockName.Equals(s)));
        }

        [Test]
        public void GetStockByNameController_PassCase()
        {
            var stock = stockController.GetStockByName("DUMMY");
            ObjectResult result = stock as ObjectResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetStockByNameController_FailCase()
        {
            var stock = stockController.GetStockByName("ABC");
            ObjectResult result = stock as ObjectResult;
            Assert.AreEqual(404, result.StatusCode);
        }

        [Test]
        public void GetStockByNameProvider_PassCase()
        {
            var stock = stockProvider.GetStockByNameProvider("DUMMY");
            Assert.IsNotNull(stock);
        }

        [Test]
        public void GetStockByNameProvider_FailCase()
        {
            var stock = stockProvider.GetStockByNameProvider("ABC");
            Assert.IsNull(stock);
        }

    }
}