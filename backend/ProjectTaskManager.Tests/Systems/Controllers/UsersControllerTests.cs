using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using ProjectTaskManager.Tests.MockData;

namespace ProjectTaskManager.Tests.Systems.Controllers;

public class UsersControllerTests
{
    // inject fake logged-in user
    private static void SetUser(UsersController controller, int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    // GET /users
    [Fact]
    public async Task GetUsers_ShouldReturn200_WithUserList()
    {
        // Arrange
        var userService = new Mock<IUsersService>();
        userService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(UserMockData.GetUsers());
        var controller = new UsersController(userService.Object);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeAssignableTo<List<UserResponce>>()
                .Which.Should().HaveCount(3);
    }

    // POST /users (Admin only)

    [Fact]
    public async Task CreateUser_ShouldReturn200_WithCreatedUser()
    {
        // Arrange
        var userService = new Mock<IUsersService>();
        var dto = new UserResponce
        {
            Username = "newuser",
            Password = "pass123",
            Email    = "new@test.com",
            Role     = "User",
            IsActive = true
        };
        var createdUser = new Users { Id = 5, Username = "newuser", Email = "new@test.com" };
        userService.Setup(s => s.AddUsersAsync(It.IsAny<Users>())).ReturnsAsync(createdUser);
        var controller = new UsersController(userService.Object);
        SetUser(controller, 1, "Admin");

        // Act
        var result = await controller.CreateUser(dto);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeOfType<Users>()
                .Which.Username.Should().Be("newuser");
    }

    // PUT /users/{id} (Admin only)

    [Fact]
    public async Task UpdateUser_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var userService = new Mock<IUsersService>();
        var dto = UserMockData.GetSingleUser(1);
        userService.Setup(s => s.UpdateUserAysnc(1, dto)).ReturnsAsync(true);
        var controller = new UsersController(userService.Object);
        SetUser(controller, 1, "Admin");

        // Act
        var result = await controller.UpdateUser(1, dto);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().Be("User updated successfully");
    }

    // DELETE /users/{id} (Admin only)

    [Fact]
    public async Task DeleteUser_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var userService = new Mock<IUsersService>();
        userService.Setup(s => s.DeleteUsersAysnc(1)).ReturnsAsync(true);
        var controller = new UsersController(userService.Object);
        SetUser(controller, 1, "Admin");

        // Act
        var result = await controller.DeleteUser(1);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().Be("User deleted successfully");
    }

    // PATCH /users/change-password

    [Fact]
    public async Task ChangePassword_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var userService = new Mock<IUsersService>();
        var dto = new ChangePasswordDto { CurrentPassword = "old123", NewPassword = "new456" };
        userService.Setup(s => s.ChangePasswordAsync(1, dto)).Returns(Task.CompletedTask);
        var controller = new UsersController(userService.Object);
        SetUser(controller, 1, "User");

        // Act
        var result = await controller.ChangePassword(dto);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().Be("Password changed successfully.");
    }
}