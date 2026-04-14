using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Identity;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;
using Projecttaskmanager.Services;
using ProjectTaskManager.Tests.MockData;

namespace ProjectTaskManager.Tests.Systems.Services;

public class UserServicesTests
{
    // ──────────────────────────────────────────────
    // GetAllUsersAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(UserMockData.GetUsers());
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserResponce>());
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.GetAllUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // ──────────────────────────────────────────────
    // GetUserByIdAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var expected = UserMockData.GetSingleUser(1);
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expected);
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.GetUserByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Username.Should().Be("tulasi");
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((UserResponce?)null);
        var service = new UsersServices(repo.Object);

        // Act
        var act = async () => await service.GetUserByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    // ──────────────────────────────────────────────
    // AddUsersAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task AddUsersAsync_ShouldReturnCreatedUser()
    {
        // Arrange
        var newUser = UserMockData.GetUserEntity();
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.AddAsync(newUser)).ReturnsAsync(newUser);
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.AddUsersAsync(newUser);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(newUser.Id);
        result.Email.Should().Be(newUser.Email);
    }

    // ──────────────────────────────────────────────
    // DeleteUsersAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task DeleteUsersAsync_ShouldReturnTrue_WhenUserDeleted()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.DeleteUsersAysnc(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUsersAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.DeleteUsersAysnc(99);

        // Assert
        result.Should().BeFalse();
    }

    // ──────────────────────────────────────────────
    // UpdateUserAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnTrue_WhenUpdateSucceeds()
    {
        // Arrange
        var dto = UserMockData.GetSingleUser(1);
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.UpdateAsync(1, dto)).ReturnsAsync(true);
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.UpdateUserAysnc(1, dto);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        // Arrange
        var dto = UserMockData.GetSingleUser(1);
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.UpdateAsync(99, dto)).ReturnsAsync(false);
        var service = new UsersServices(repo.Object);

        // Act
        var result = await service.UpdateUserAysnc(99, dto);

        // Assert
        result.Should().BeFalse();
    }

    // ──────────────────────────────────────────────
    // ChangePasswordAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task ChangePasswordAsync_ShouldHashAndSave_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var user = UserMockData.GetUserEntity();

        // Pre-hash so VerifyHashedPassword passes inside the service
        var hasher = new PasswordHasher<Users>();
        user.PasswordHash = hasher.HashPassword(user, "CurrentPass1!");

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "CurrentPass1!",
            NewPassword     = "NewPass2@"
        };

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(user);
        repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        var service = new UsersServices(repo.Object);

        // Act
        var act = async () => await service.ChangePasswordAsync(1, dto);

        // Assert
        await act.Should().NotThrowAsync();
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        user.PasswordHash.Should().NotBe("hashed_password_123");
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldThrowKeyNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetEntityByIdAsync(99)).ReturnsAsync((Users?)null);
        var service = new UsersServices(repo.Object);

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "any",
            NewPassword     = "NewPass2@"
        };

        // Act
        var act = async () => await service.ChangePasswordAsync(99, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("User not found.");
    }

    [Fact]
    public async Task ChangePasswordAsync_ShouldThrowUnauthorizedAccessException_WhenCurrentPasswordIsWrong()
    {
        // Arrange
        var user = UserMockData.GetUserEntity();
        var hasher = new PasswordHasher<Users>();
        user.PasswordHash = hasher.HashPassword(user, "CorrectPass1!");

        var dto = new ChangePasswordDto
        {
            CurrentPassword = "WrongPass!",
            NewPassword     = "NewPass2@"
        };

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetEntityByIdAsync(1)).ReturnsAsync(user);
        var service = new UsersServices(repo.Object);

        // Act
        var act = async () => await service.ChangePasswordAsync(1, dto);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Current password is incorrect.");
    }

    // ──────────────────────────────────────────────
    // ResetPasswordAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task ResetPasswordAsync_ShouldHashAndSave_WhenUserExists()
    {
        // Arrange
        var user = UserMockData.GetUserEntityByEmail("Tulasi@test.com");

        var dto = new ResetPasswordDto
        {
            Email       = "Tulasi@test.com",
            NewPassword = "ResetPass3#"
        };

        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync("Tulasi@test.com")).ReturnsAsync(user);
        repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        var service = new UsersServices(repo.Object);

        // Act
        var act = async () => await service.ResetPasswordAsync(dto);

        // Assert
        await act.Should().NotThrowAsync();
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        user.PasswordHash.Should().NotBe("hashed_password_123");
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldThrowKeyNotFoundException_WhenEmailNotFound()
    {
        // Arrange
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync("unknown@test.com")).ReturnsAsync((Users?)null);
        var service = new UsersServices(repo.Object);

        var dto = new ResetPasswordDto
        {
            Email       = "unknown@test.com",
            NewPassword = "ResetPass3#"
        };

        // Act
        var act = async () => await service.ResetPasswordAsync(dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("User not found.");
    }
}