using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using System.Security.Claims;
using ProjectTaskManager.Tests.MockData;
using Microsoft.AspNetCore.Http;

namespace ProjectTaskManager.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly UsersController _controller;
        private readonly Mock<IUsersService> _mockService;

        public UsersControllerTests()
        {
            _mockService = new Mock<IUsersService>();
            _controller = new UsersController(_mockService.Object);
        }

        // Get all users
        [Fact]
        public async Task GetUsers_ReturnsOk()
        {

            var users = UserMockData.GetUsers();
            _mockService.Setup(x => x.GetAllUsersAsync())
                        .ReturnsAsync(users);

            var result = await _controller.GetUsers();

            Assert.IsType<OkObjectResult>(result.Result);
        }

        // Get user by id
        //we use claims here cause we should get the logged-in user
        [Fact]
        public async Task GetUser_ReturnsOk()
        {
            var user = UserMockData.GetSingleUser(1);
            _mockService.Setup(x => x.GetUserByIdAsync(1))
                        .ReturnsAsync(user);
            // fake logged-in user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");//user is authenticated
            var principal = new ClaimsPrincipal(identity);
            //letting the controller think that the user is logged in
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
            var result = await _controller.GetUser(1);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        // Create user
        [Fact]
        public async Task CreateUser_ReturnsOk()
        {
            var dto = UserMockData.GetSingleUser(1);
            var userEntity = UserMockData.GetUserEntity();

            _mockService.Setup(x => x.AddUsersAsync(It.IsAny<Users>()))
                        .ReturnsAsync(userEntity);

            var result = await _controller.CreateUser(dto);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        // Delete user
        [Fact]
        public async Task DeleteUser_ReturnsOk()
        {
            _mockService.Setup(x => x.DeleteUsersAysnc(1))
                        .ReturnsAsync(true);

            var result = await _controller.DeleteUser(1);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}