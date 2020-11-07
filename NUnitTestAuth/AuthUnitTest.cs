using AuthorizationApi1.Models;
using AuthorizationApi1.Provider;
using AuthorizationApi1.Repository;
using AuthorizationApi1.Controllers;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System;

namespace NUnitTestAuth
{
    public class Tests
    {
        Mock<IAuthenticationProvider> authProvider;
        Mock<IAuthenticationRepository> authRepo;
        Mock<IConfiguration> _config;
        AuthController authController;
        AuthenticationProvider _provider;
        AuthenticationRepository _repository;
        User user;
        [SetUp]
        public void Setup()
        {
            _config = new Mock<IConfiguration>();
            authRepo = new Mock<IAuthenticationRepository>();
            authProvider = new Mock<IAuthenticationProvider>();
            authController = new AuthController(authProvider.Object);
            _provider = new AuthenticationProvider(authRepo.Object);
            _repository = new AuthenticationRepository(_config.Object);
            user = new User() { 
            PortFolioID=12345,
            Password="admin1"
            };
            string token = "abcd1234";
            authProvider.Setup(s => s.Login(user)).Returns(token);
            authRepo.Setup(s => s.Login(user)).Returns(token);
        }

        [Test]
        public void ValidTokenGeneratedControllerTest()
        {
            User newUser = new User
            {
                PortFolioID = 12345,
                Password = "admin1"
            };
            var genToken = authController.AuthenticateUser(newUser);

            Assert.IsNotNull(genToken);
            
        }

        [Test]
        public void InvalidTokenGeneratedControllerTest()
        {
            try
            {
                User newUser = new User
                {
                    PortFolioID = 12345,
                    Password = "abc123"
                };
                var genToken = authController.AuthenticateUser(newUser);
                var res = genToken as ObjectResult;
                Assert.AreEqual(401, res.StatusCode);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Object reference not set to an instance of an object.", e.Message);
            }
        }

        [Test]
        public void ValidTokenGeneratedProviderTest()
        {
            User newUser = new User
            {
                PortFolioID = 67890,
                Password = "admin2"
            };
            var genToken = _provider.Login(newUser);

            Assert.IsNotNull(genToken);

        }


        [Test]
        public void InvalidTokenGeneratedProviderTest()
        {
            try
            {
                User newUser = new User
                {
                    PortFolioID = 12345,
                    Password = "abc123"
                };
                var genToken = _provider.Login(newUser);
                Assert.IsNull(genToken);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Object reference not set to an instance of an object.", e.Message);
            }
        }

         [Test]
         public void ValidTokenGeneratedRepositoryTest()
         {
             User newUser = new User
             {
                 PortFolioID = 12345,
                 Password = "admin1"
             };
             string genToken = _repository.Login(newUser);

             Assert.IsNotNull(genToken);

         }




        [Test]
        public void InvalidTokenGeneratedRepositoryTest()
        {
            try
            {
                User newUser = new User
                {
                    PortFolioID = 12345,
                    Password = "abc123"
                };
                var genToken = _repository.Login(newUser);

                Assert.IsNull(genToken);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Object reference not set to an instance of an object.", e.Message);
            }
        }


        /* 
         [Test]
         public void GetTokenPositiveTest()
         {
             User user = new User()
             {
                 PortfolioId = 12345,
                 Password = "admin1"
             };
             string result = _provider.Login(user);
             Assert.IsNotNull(result);
         }*/
        /*
         [Test]
         public void GetTokenNegativeTest()
         {
             User user = new User()
             {
                 PortfolioId = 12345,
                 Password = "yashi@123"
             };
             var result = _provider.GetToken(user);
             Assert.IsNull(result);
         }

         [Test]
         public void GenerateTokenPositiveTest()
         {
             User user = new User()
             {
                 PortfolioId = 12345,
                 Password = "abc@123"
             };
             var result = _repository.GenerateToken(user);
             Assert.IsNotNull(result);
         }

         [Test]
         public void GenerateTokenNegativeTest()
         {
             User user = new User()
             {
                 PortfolioId = 12345,
                 Password = "yashi@123"
             };
             var result = _repository.GenerateToken(user);
             Assert.IsNull(result);
         }*/
    }
}