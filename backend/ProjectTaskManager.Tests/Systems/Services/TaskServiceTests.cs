using FluentAssertions;
using Moq;
using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;
using Projecttaskmanager.Services;
using ProjectTaskManager.MockData;

namespace ProjectTaskManager.Tests.Systems.Services;

public class TaskServiceTests
{
    // ──────────────────────────────────────────────
    // GetAllTasksAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(TaskMockData.GetTasks());
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.GetAllTasksAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ProjectTasks>());
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.GetAllTasksAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // ──────────────────────────────────────────────
    // GetTasksByIdAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetTasksByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var expected = TaskMockData.GetSingleTask(1);
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expected);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.GetTasksByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Title.Should().Be("Create Tables");
    }

    [Fact]
    public async Task GetTasksByIdAsync_ShouldThrowKeyNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ProjectTasks?)null);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var act = async () => await service.GetTasksByIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    // ──────────────────────────────────────────────
    // GetTasksByProjectId
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetTasksByProjectId_ShouldReturnTasksForProject()
    {
        // Arrange
        var expected = TaskMockData.GetTasksByProject(1);
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetByProjectIdAsync(1)).ReturnsAsync(expected);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.GetTasksByProjectId(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().OnlyContain(t => t.ProjectId == 1);
    }

    // ──────────────────────────────────────────────
    // AddTasksAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task AddTasksAsync_ShouldReturnCreatedTask()
    {
        // Arrange
        var newTask = TaskMockData.GetSingleTask(1);
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.AddAsync(newTask)).ReturnsAsync(newTask);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.AddTasksAsync(newTask);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(newTask.Id);
        result.Title.Should().Be(newTask.Title);
    }

    // ──────────────────────────────────────────────
    // DeleteTaskAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnTrue_WhenTaskDeleted()
    {
        // Arrange
        var task = TaskMockData.GetSingleTask(1);
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetWithDependenciesAsync(1)).ReturnsAsync(task);
        repo.Setup(r => r.DeleteAsync(task)).Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.DeleteTaskAsync(1);

        // Assert
        result.Should().BeTrue();
        repo.Verify(r => r.DeleteAsync(task), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldThrowKeyNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetWithDependenciesAsync(99)).ReturnsAsync((ProjectTasks?)null);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var act = async () => await service.DeleteTaskAsync(99);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    // ──────────────────────────────────────────────
    // GetBlockingTasksAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetBlockingTasksAsync_ShouldReturnBlockingTasks()
    {
        // Arrange
        var blockingTasks = TaskMockData.GetTasksByProject(1);
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetBlockingTasksAsync(2)).ReturnsAsync(blockingTasks);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.GetBlockingTasksAsync(2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetBlockingTasksAsync_ShouldReturnEmptyList_WhenNoBlockingTasks()
    {
        // Arrange
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetBlockingTasksAsync(1)).ReturnsAsync(new List<ProjectTasks>());
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.GetBlockingTasksAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // ──────────────────────────────────────────────
    // GetTasksByUserIdAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task GetTasksByUserIdAsync_ShouldReturnTasksForUser()
    {
        // Arrange
        var expected = TaskMockData.GetTasksByUser(2);
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetByUserIdAsync(2)).ReturnsAsync(expected);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.GetTasksByUserIdAsync(2);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().OnlyContain(t => t.AssigneeId == 2);
    }

    // ──────────────────────────────────────────────
    // UpdateTaskAsync
    // ──────────────────────────────────────────────

    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnTrue_WhenUpdateSucceeds()
    {
        // Arrange
        var existing = TaskMockData.GetSingleTask(1); // Status = "Pending"
        var updated  = TaskMockData.GetSingleTask(1);
        updated.Status = "InProgress";

        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        repo.Setup(r => r.ExecuteUpdateAsync(1, updated)).Returns(Task.CompletedTask);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.UpdateTaskAsync(1, updated);

        // Assert
        result.Should().BeTrue();
        repo.Verify(r => r.ExecuteUpdateAsync(1, updated), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldThrowKeyNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((ProjectTasks?)null);
        var service = new TaskService(repo.Object, notificationService.Object);

        var updatedTask = TaskMockData.GetSingleTask(1);

        // Act
        var act = async () => await service.UpdateTaskAsync(99, updatedTask);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*99*");
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldSendNotifications_WhenTaskJustCompleted()
    {
        // Arrange
        var existing = TaskMockData.GetSingleTask(1); // Status = "Pending"
        existing.Status = "InProgress";

        var updated = TaskMockData.GetSingleTask(1);
        updated.Status = "Completed"; // transition: InProgress → Completed

        var dependentTasks = TaskMockData.GetTasksByUser(3); // tasks assigned to user 3

        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();

        repo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        repo.Setup(r => r.ExecuteUpdateAsync(1, updated)).Returns(Task.CompletedTask);
        repo.Setup(r => r.GetDependentTasksAsync(1)).ReturnsAsync(dependentTasks);
        notificationService
            .Setup(n => n.CreateNotification(It.IsAny<int>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.UpdateTaskAsync(1, updated);

        // Assert
        result.Should().BeTrue();
        notificationService.Verify(
            n => n.CreateNotification(It.IsAny<int>(), It.IsAny<string>()),
            Times.Exactly(dependentTasks.Count)
        );
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldNotSendNotifications_WhenTaskWasAlreadyCompleted()
    {
        // Arrange
        var existing = TaskMockData.GetSingleTask(3); // Status = "Completed"
        var updated  = TaskMockData.GetSingleTask(3);
        updated.Status = "Completed"; // no transition — was already completed

        var repo = new Mock<ITaskRepository>();
        var notificationService = new Mock<INotificationService>();
        repo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(existing);
        repo.Setup(r => r.ExecuteUpdateAsync(3, updated)).Returns(Task.CompletedTask);
        var service = new TaskService(repo.Object, notificationService.Object);

        // Act
        var result = await service.UpdateTaskAsync(3, updated);

        // Assert
        result.Should().BeTrue();
        notificationService.Verify(
            n => n.CreateNotification(It.IsAny<int>(), It.IsAny<string>()),
            Times.Never
        );
    }
}