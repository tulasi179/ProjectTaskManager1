using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Services;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Models;  
using Projecttaskmanager.DTOs; 
using ProjectTaskManager.Tests.MockData;

namespace ProjectTaskManager.Tests.Systems.controller
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IAuthService> _mockService;

        public AuthControllerTests()
        {
            _mockService = new Mock<IAuthService>();
            _controller = new AuthController(_mockService.Object);
        }

        // register success
        [Fact]
        public async Task Register_ReturnsOk()
        {
            var request = AuthMockData.ValidUser();
            var user = new Users { Username = "test" };
            _mockService.Setup(x => x.RegisterAsync(request))
                        .ReturnsAsync(user);

            var result = await _controller.Register(request);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        // register fail
        [Fact]
        public async Task Register_ReturnsBadRequest()
        {
            var request = AuthMockData.ValidUser();
            _mockService.Setup(x => x.RegisterAsync(request))
                        .ReturnsAsync((Users)null);
            var result = await _controller.Register(request);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // Login Success
        [Fact]
        public async Task Login_ReturnsOk()
        {
            var request = AuthMockData.ValidUser();
            var token = new TokenResponce
            {
                AccessToken = "test-token",
                RefreshToken = "test-refresh"
            };

            // ↓ updated to return tuple
            _mockService.Setup(x => x.LoginAsync(request))
                        .ReturnsAsync((token, (string?)null));

            var result = await _controller.Login(request);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        // Login Fail
        [Fact]
        public async Task Login_ReturnsBadRequest()
        {
            var request = new UserResponce
            {
                Username = "wrong",
                Password = "wrong"
            };

            // ↓ updated to return tuple
            _mockService.Setup(x => x.LoginAsync(request))
                        .ReturnsAsync(((TokenResponce?)null, "Invalid username or password."));

            var result = await _controller.Login(request);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}