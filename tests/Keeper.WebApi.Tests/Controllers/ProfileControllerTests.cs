using NUnit.Framework;
using Keeper.WebApi.Controllers;
using Moq;
using Keeper.WebApi.Services;
using System.Threading.Tasks;
using Shouldly;
using Microsoft.AspNetCore.Mvc;
using System;
using Keeper.WebApi.Models;

namespace Keeper.WebApi.Tests.Controllers
{
    public class ProfileControllerTests
    {
        private Mock<IUsersService> usersServiceMock;

        [SetUp]
        public void Setup()
        {
            usersServiceMock = new Mock<IUsersService>();
        }

        [Test]
        public async Task CreateAsyncWhenSecretIsProvidedShouldCreateUser()
        {
            usersServiceMock.Setup(s => s.CreateAsync("secret")).Returns(Task.FromResult("key"));
            var controller = new ProfileController(usersServiceMock.Object);

            var result = (OkObjectResult)await controller.CreateAsync("secret");

            result.ShouldNotBeNull();
            result.Value.ShouldBe("key");
        }

        [Test]
        public async Task CreateAsyncWhenSecretIsMissingShouldReturnBadRequest()
        {
            var controller = new ProfileController(usersServiceMock.Object);

            var result = (BadRequestObjectResult)await controller.CreateAsync("");

            result.ShouldNotBeNull();
            result.Value.ShouldBe("Missing profile's secret");
        }

        [Test]
        public async Task CreateAsyncWhenSavingFailsShouldReturnStatusCode500()
        {
            usersServiceMock.Setup(s => s.CreateAsync("secret")).Returns(Task.FromResult(""));
            var controller = new ProfileController(usersServiceMock.Object);

            var result = (ObjectResult)await controller.CreateAsync("secret");

            result.ShouldNotBeNull();
            result.StatusCode.Value.ShouldBe(500);
            result.Value.ShouldBe("Unable to create new synchronization profile");
        }

        [Test]
        public async Task ExistsAsyncWhenModelStateIsNotValidShouldReturnBadRequest()
        {
            var controller = new ProfileController(usersServiceMock.Object);
            controller.ModelState.AddModelError("Model error", "Error message");

            var result = (BadRequestObjectResult)await controller.ExistsAsync("");

            result.ShouldNotBeNull();
        }

        [TestCase(true, typeof(OkResult))]
        [TestCase(false, typeof(NotFoundResult))]
        public async Task ExistsAsyncWhenKeyIsProvidedShouldReturnCorrectResult(bool exists, Type returnType)
        {
            usersServiceMock.Setup(s => s.ExistsAsync("key")).Returns(Task.FromResult(exists));
            var controller = new ProfileController(usersServiceMock.Object);

            var result = await controller.ExistsAsync("key");

            result.ShouldBeOfType(returnType);
        }

        [Test]
        public async Task LoginAsyncWhenModelStateIsNotValidShouldReturnBadRequest()
        {
            var controller = new ProfileController(usersServiceMock.Object);
            controller.ModelState.AddModelError("Error", "Error");

            var result = (BadRequestObjectResult)await controller.LoginAsync(null);

            result.ShouldNotBeNull();
        }

        [TestCase(null, "secret")]
        [TestCase("key", null)]
        public async Task LoginAsyncWhenLoginParametersAreMissinShouldReturnBadRequest(string key, string secret)
        {
            var loginObj = new LoginForm()
            {
                Key = key,
                Secret = secret
            };
            var controller = new ProfileController(usersServiceMock.Object);

            var result = (BadRequestObjectResult)await controller.LoginAsync(loginObj);

            result.ShouldNotBeNull();
            result.Value.ShouldBe("Login object must have key and secret properties");
        }

        [Test]
        public async Task LoginAsyncWhenLoginCredentialsAreIncorrectShouldReturnUnauthorized()
        {
            var loginObj = new LoginForm()
            {
                Key = "key",
                Secret = "secret"
            };
            usersServiceMock.Setup(s => s.LoginAsync(loginObj)).Returns(Task.FromResult(string.Empty));
            var controller = new ProfileController(usersServiceMock.Object);

            var result = (UnauthorizedObjectResult)await controller.LoginAsync(loginObj);

            result.ShouldNotBeNull();
            result.Value.ShouldBe("Incorrect credentials");
        }

        [Test]
        public async Task LoginAsyncWhenLoginCredentialsAreCorrectShouldReturnOk()
        {
            var loginObj = new LoginForm()
            {
                Key = "key",
                Secret = "secret"
            };
            usersServiceMock.Setup(s => s.LoginAsync(loginObj)).Returns(Task.FromResult("token"));
            var controller = new ProfileController(usersServiceMock.Object);

            var result = (OkObjectResult)await controller.LoginAsync(loginObj);

            result.ShouldNotBeNull();
            result.Value.ShouldBe("token");
        }
    }
}