using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using ProjectTaskManager.MockData;

namespace ProjectTaskManager.Tests.Systems.Controllers;

public class TaskDependencyControllerTests
{
    // GET /taskdependency
    [Fact]
    public async Task GetDependencies_ShouldReturn200_WithDependencyList()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        dependencyService.Setup(s => s.GetDependencies()).ReturnsAsync(TaskDependencyMockData.GetDependencies());
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.GetDependencies();

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeAssignableTo<List<TaskDependency>>()
                .Which.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetDependencies_WhenNoneExist_ShouldReturn200_WithEmptyList()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        dependencyService.Setup(s => s.GetDependencies()).ReturnsAsync(new List<TaskDependency>());
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.GetDependencies();

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<List<TaskDependency>>()
                .Which.Should().BeEmpty();
    }
// GET /taskdependency/{taskId}/dependents
[Fact]
    public async Task GetDependentTasksById_ShouldReturn200_WithMatchingDependencies()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        var dependents = TaskDependencyMockData.GetDependentTasksById(1); // 2 entries for taskId 1
        dependencyService.Setup(s => s.GetDependentTasksById(1)).ReturnsAsync(dependents);
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.GetDependentTasksById(1);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeAssignableTo<List<TaskDependency>>()
                .Which.Should().HaveCount(2)
                .And.OnlyContain(d => d.TaskId == 1);
    }

    [Fact]
    public async Task GetDependentTasksById_WhenNoneExist_ShouldReturn200_WithEmptyList()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        dependencyService.Setup(s => s.GetDependentTasksById(99)).ReturnsAsync(new List<TaskDependency>());
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.GetDependentTasksById(99);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<List<TaskDependency>>()
                .Which.Should().BeEmpty();
    }

   
    // POST /taskdependency
[Fact]
    public async Task AddDependency_ValidRequest_ShouldReturn201_WithCreatedDependency()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        var dto = new TaskDependencyRequestDto { TaskId = 1, DependentTaskId = 2 };
        var created = TaskDependencyMockData.GetSingleDependency(1, 2);
        dependencyService
            .Setup(s => s.AddDependency(It.IsAny<TaskDependency>()))
            .ReturnsAsync((true, "Created", created));
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.AddDependency(dto);

        // Assert
        var created201 = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created201.StatusCode.Should().Be(201);
        created201.Value.Should().BeOfType<TaskDependency>()
                .Which.DependentTaskId.Should().Be(2);
    }

    [Fact]
    public async Task AddDependency_CircularDependency_ShouldReturn400_WithMessage()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        var dto = new TaskDependencyRequestDto { TaskId = 1, DependentTaskId = 1 };
        dependencyService
            .Setup(s => s.AddDependency(It.IsAny<TaskDependency>()))
            .ReturnsAsync((false, "Circular dependency detected", null));
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.AddDependency(dto);

        // Assert
        var bad = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        bad.StatusCode.Should().Be(400);
        bad.Value.Should().BeEquivalentTo(new { message = "Circular dependency detected" });
    }

    [Fact]
    public async Task AddDependency_DuplicateDependency_ShouldReturn400_WithMessage()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        var dto = new TaskDependencyRequestDto { TaskId = 1, DependentTaskId = 2 };
        dependencyService
            .Setup(s => s.AddDependency(It.IsAny<TaskDependency>()))
            .ReturnsAsync((false, "Dependency already exists", null));
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.AddDependency(dto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
              .Which.Value.Should().BeEquivalentTo(new { message = "Dependency already exists" });
    }

    // DELETE /taskdependency/{taskId}/{dependentTaskId}

    [Fact]
    public async Task RemoveDependency_WhenExists_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        dependencyService.Setup(s => s.RemoveDependency(1, 2)).ReturnsAsync(true);
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.RemoveDependency(1, 2);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().Be("Dependency removed successfully");
    }

    [Fact]
    public async Task RemoveDependency_WhenNotFound_ShouldReturn404()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        dependencyService.Setup(s => s.RemoveDependency(99, 100)).ReturnsAsync(false);
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        var result = await controller.RemoveDependency(99, 100);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
              .Which.Value.Should().Be("Dependency not found");
    }

    [Fact]
    public async Task RemoveDependency_CallsServiceWithCorrectIds()
    {
        // Arrange
        var dependencyService = new Mock<ITaskDependencyService>();
        dependencyService.Setup(s => s.RemoveDependency(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
        var controller = new TaskDependencyController(dependencyService.Object);

        // Act
        await controller.RemoveDependency(1, 3);

        // Assert — verify the service was called with exactly the right IDs
        dependencyService.Verify(s => s.RemoveDependency(1, 3), Times.Once);
    }
}