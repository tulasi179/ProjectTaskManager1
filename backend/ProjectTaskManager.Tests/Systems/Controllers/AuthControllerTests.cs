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
        private readonly Mock<IAuthService> _mockService;// fake version of the service form the main project
        public AuthControllerTests()
        {
            _mockService = new Mock<IAuthService>();//create fake service
            _controller = new AuthController(_mockService.Object);//gives fake service to the controllers
        }

        //  register success
        [Fact]
        public async Task Register_ReturnsOk()
        {
            //input dtaa
            var request = AuthMockData.ValidUser();
            //expected output data
            var user = new Users { Username = "test" };
            //if we give the fake data (request) to the registerrAsync the the output should be user
            _mockService.Setup(x => x.RegisterAsync(request))
                        .ReturnsAsync(user);//to see if it works make user as (Users)null.

            var result = await _controller.Register(request);
            //assert
            //checks if the result is 200 ok.
            Assert.IsType<OkObjectResult>(result.Result);
        }

        //  register fail
        [Fact]
        public async Task Register_ReturnsBadRequest()
        {
            var request = AuthMockData.ValidUser();
            _mockService.Setup(x => x.RegisterAsync(request))
                        .ReturnsAsync((Users)null);
            var result = await _controller.Register(request);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        //  Login Success
        [Fact]
        public async Task Login_ReturnsOk()
        {
            var request = AuthMockData.ValidUser();
          var token = new TokenResponce
            {
                AccessToken = "test-token",
                RefreshToken = "test-refresh"
            }; 
            _mockService.Setup(x => x.LoginAsync(request))
                        .ReturnsAsync(token);
            var result = await _controller.Login(request);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        //  Login Fail
        [Fact]
        public async Task Login_ReturnsBadRequest()
        {
            
            var request = new UserResponce
            {
                Username = "wrong",
                Password = "wrong"
            };
            //Inline data 
            //instead we can also use this line .
            //var request  = AuthMockData.InValidData();
            _mockService.Setup(x => x.LoginAsync(request))
                        .ReturnsAsync((TokenResponce)null);
            var result = await _controller.Login(request);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}