using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.Models;
using Projecttaskmanager.DTOs;

public class TaskControllerTests
{
    private readonly Mock<ITaskService> _service;
    private readonly TaskController _controller;

    public TaskControllerTests()
    {
        _service = new Mock<ITaskService>();
        _controller = new TaskController(_service.Object);
    }

    // Test: GetTasks returns Ok
    [Fact]
    public async Task GetTasks_ReturnsOk()
    {
        _service.Setup(s => s.GetAllTasksAsync())
                .ReturnsAsync(new List<ProjectTasks>());

        var result = await _controller.GetTasks();

        Assert.IsType<OkObjectResult>(result.Result);
    }

    // Test: GetTask by Id
    [Fact]
    public async Task GetTask_ReturnsOk()
    {
        _service.Setup(s => s.GetTasksByIdAsync(1))
                .ReturnsAsync(new ProjectTasks { Id = 1 });

        var result = await _controller.GetTask(1);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    //  Test: CreateTask
    [Fact]
    public async Task CreateTask_ReturnsOk()
    {
        _service.Setup(s => s.AddTasksAsync(It.IsAny<ProjectTasks>()))
                .ReturnsAsync(new ProjectTasks { Id = 1 });

        var controller = new TaskController(_service.Object);

        var dto = new TaskRequestDto
        {
            ProjectId = 1,
            Title = "Test",
            Description = "Test",
            AssigneeId = 2
        };

        var result = await controller.CreateTask(dto);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    //test: DeleteTask
    [Fact]
    public async Task DeleteTask_ReturnsOk()
    {
        var result = await _controller.DeleteTask(1);

        Assert.IsType<OkObjectResult>(result);
    }
}