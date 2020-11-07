using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using PortfolioAuthorizationApi.Controllers;
using PortfolioAuthorizationApi.Models;
using PortfolioAuthorizationApi.Provider;
using PortfolioAuthorizationApi.Repository;

namespace PortfolioAuthNUnitTesting
{
    public class Tests
    {
        private Mock<IConfiguration> _config;
        private IAuthRepository _repository;
        private IAuthProvider _provider;
        private AuthController _controller;
        User user1;
        User user2;

        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>();
            _config.Setup(c => c["Jwt:Key"]).Returns("ThisismySecretKey");
            _repository = new AuthRepository(_config.Object);
            _provider = new AuthProvider(_repository);
            _controller = new AuthController(_provider);
             user1 = new User()
            {
                PortFolioID = 12345,
                Password = "admin1"
            };
            user2 = new User()
            {
                PortFolioID = 12345,
                Password = "admin5"
            };


        }

        [Test]
        public void AuthenticateUserControllerPositiveTest()
        {
            var result = _controller.AuthenticateUser(user1);
            Assert.IsNotNull(result);
        }

        [Test]
        public void AuthenticateUserControllerNegativeTest()
        {
           
            try
            {
                var result = _controller.AuthenticateUser(user2);
                var response = result as ObjectResult;
                Assert.AreEqual(401, response.StatusCode);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Object reference not set to an instance of an object.", e.Message);
            }
        }

        [Test]
        public void AuthenticateUserProviderPositiveTest()
        {
            var result = _provider.AuthenticateUser(user1);
            Assert.IsNotNull(result);
        }

        [Test]
        public void AuthenticateUserProviderNegativeTest()
        {
            var result = _provider.AuthenticateUser(user2);
            Assert.IsNull(result);
        }

        [Test]
        public void GenerateTokenRepositoryPositiveTest()
        {
            var result = _repository.GenerateToken(user1);
            Assert.IsNotNull(result);
        }

        [Test]
        public void GenerateTokenRepositoryNegativeTest()
        {
            var result = _repository.GenerateToken(user2);
            Assert.IsNull(result);
        }
    }
}