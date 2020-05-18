using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Keeper.WebApi.Controllers;
using Keeper.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Keeper.WebApi.Tests.Controllers
{
    class TransactionsControllerTests
    {
        private Mock<ITransactionsService> transactionsServiceMock;

        [SetUp]
        public void Setup()
        {
            transactionsServiceMock = new Mock<ITransactionsService>();
        }

        [Test]
        public async Task AddWhenModelStateIsNotValidShouldReturnBadRequest()
        {
            var controller = TransactionsController(transactionsServiceMock.Object);
            controller.ModelState.AddError("Error", "Error");

            var result = (BadRequestObjectResult)await controller.Add("transactions");

            result.ShouldNotBeNull();
        }

        [Test]
        public async Task AddWhenUserIsNotAuthenticatedShouldReturnUnauthorized()
        {
            var user = new ClaimsPrincipal();
            var controller = TransactionsController(transactionsServiceMock.Object);
            controller.User = user;

            var result = (UnauthorizedObjectResult)await controller.Add("transactions");

            result.ShouldNotBeNull();
            result.Value.ShouldBe("Token is invalid");
        }

        [Test]
        public async Task AddWhenSavingFailedShouldReturnStatusCode500()
        {
            transactionsServiceMock.Setup(s => s.AddAsync("user", "transactions")).Returns(-1);
            var user = new ClaimsPrincipal(new List<Claim>() { new Claim(ClaimTypes.Name, "user")});
            var controller = TransactionsController(transactionsServiceMock.Object);
            controller.User = user;

            var result = (ObjectResult)await controller.Add("transactions");

            result.ShouldNotBeNull();
            result.StatusCode.Value.ShouldBe(500);
            result.Value.ShouldBe("Error has occured while adding transaction");
        }

        [Test]
        public async Task AddWhenTransactionIsAddedShouldReturnOk()
        {
            
            transactionsServiceMock.Setup(s => s.AddAsync("user", "transactions")).Returns(10);
            var user = new ClaimsPrincipal(new List<Claim>() { new Claim(ClaimTypes.Name, "user")});
            var controller = TransactionsController(transactionsServiceMock.Object);
            controller.User = user;

            var result = (OkObjectResult)await controller.Add("transactions");

            result.ShouldNotBeNull();
            result.Value.ShouldBe(10);
        }
    }
}
