using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.Models;
using Projecttaskmanager.DTOs;

public class TaskDependencyControllerTests
{
    private readonly Mock<ITaskDependencyService> _service;
    private readonly TaskDependencyController _controller;

    public TaskDependencyControllerTests()
    {
        _service = new Mock<ITaskDependencyService>();
        _controller = new TaskDependencyController(_service.Object);
    }

    // Get all dependencies
    [Fact]
    public async Task GetDependencies_ReturnsOk()
    {
        _service.Setup(s => s.GetDependencies())
                .ReturnsAsync(new List<TaskDependency>());

        var result = await _controller.GetDependencies();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // Get dependents by taskId
    [Fact]
    public async Task GetDependentTasksById_ReturnsOk()
    {
        _service.Setup(s => s.GetDependentTasksById(1))
                .ReturnsAsync(new List<TaskDependency>());

        var result = await _controller.GetDependentTasksById(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    //Add dependency (success)
    [Fact]
    public async Task AddDependency_ReturnsCreated()
    {
        _service.Setup(s => s.AddDependency(It.IsAny<TaskDependency>()))
                .ReturnsAsync((true, "Success", new TaskDependency { TaskId = 1 }));

        var dto = new TaskDependencyRequestDto
        {
            TaskId = 1,
            DependentTaskId = 2
        };

        var result = await _controller.AddDependency(dto);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    // Add dependency (fail)
    [Fact]
    public async Task AddDependency_ReturnsBadRequest()
    {
        _service.Setup(s => s.AddDependency(It.IsAny<TaskDependency>()))
                .ReturnsAsync((false, "Error", null));

        var dto = new TaskDependencyRequestDto
        {
            TaskId = 1,
            DependentTaskId = 2
        };

        var result = await _controller.AddDependency(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Delete dependency
    [Fact]
    public async Task RemoveDependency_ReturnsOk()
    {
        _service.Setup(s => s.RemoveDependency(1, 2))
                .ReturnsAsync(true);

        var result = await _controller.RemoveDependency(1, 2);

        Assert.IsType<OkObjectResult>(result);
    }

    // Delete dependency not found
    [Fact]
    public async Task RemoveDependency_ReturnsNotFound()
    {
        _service.Setup(s => s.RemoveDependency(1, 2))
                .ReturnsAsync(false);

        var result = await _controller.RemoveDependency(1, 2);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}