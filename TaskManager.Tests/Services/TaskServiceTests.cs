using FluentAssertions;
using Moq;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Application.Validators;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;
using TaskManager.Tests.Common;

namespace TaskManager.Tests.Services
{
    public class TaskServiceTests
    {
        // ── shared mocks — rebuilt fresh for every test ──────────────────
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<ITaskRepository> _taskRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IProjectRepository> _projectRepoMock;
        private readonly TaskService _sut;   // sut = System Under Test

        public TaskServiceTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _taskRepoMock = new Mock<ITaskRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _projectRepoMock = new Mock<IProjectRepository>();

            // Wire mocks into UoW
            _uowMock.Setup(u => u.Tasks).Returns(_taskRepoMock.Object);
            _uowMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
            _uowMock.Setup(u => u.Projects).Returns(_projectRepoMock.Object);

            // Use real validators — we want to test that validation wires up correctly
            var createValidator = new CreateTaskRequestValidator();
            var updateValidator = new UpdateTaskStatusRequestValidator();

            _sut = new TaskService(_uowMock.Object, createValidator, updateValidator);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_WhenTaskExists_ReturnsSuccessWithTask()
        {
            // Arrange
            var task = TestDataBuilder.BuildTask();
            _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id))
                         .ReturnsAsync(task);

            // Act
            var result = await _sut.GetByIdAsync(task.Id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.Id.Should().Be(task.Id);
            result.Value.Title.Should().Be(task.Title);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _taskRepoMock.Setup(r => r.GetByIdAsync(nonExistentId))
                         .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _sut.GetByIdAsync(nonExistentId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain(nonExistentId.ToString());
        }

        // ── CreateAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_WithValidRequest_ReturnsSuccessWithCreatedTask()
        {
            // Arrange
            var project = TestDataBuilder.BuildProject();
            var user = TestDataBuilder.BuildUser();
            var request = TestDataBuilder.BuildCreateTaskRequest(project.Id, user.Id);

            _projectRepoMock.Setup(r => r.GetByIdAsync(request.ProjectId))
                            .ReturnsAsync(project);
            _userRepoMock.Setup(r => r.GetByIdAsync(request.AssignedToUserId))
                         .ReturnsAsync(user);
            _uowMock.Setup(u => u.SaveChangesAsync(default))
                    .ReturnsAsync(1);

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value!.Title.Should().Be(request.Title);
            result.Value.Priority.Should().Be("Medium");
            result.Value.Status.Should().Be("Todo");   // always starts as Todo

            // Verify side effects — AddAsync and SaveChanges were called exactly once
            _taskRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithEmptyTitle_ReturnsFailureWithValidationError()
        {
            // Arrange
            var request = TestDataBuilder.BuildCreateTaskRequest() with { Title = "" };

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Title is required");

            // Verify nothing was saved — validation failed before hitting the DB
            _taskRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidPriority_ReturnsFailure()
        {
            // Arrange
            var request = TestDataBuilder.BuildCreateTaskRequest() with { Priority = "SuperUrgent" };

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Priority must be");
        }

        [Fact]
        public async Task CreateAsync_WhenProjectNotFound_ReturnsFailure()
        {
            // Arrange
            var request = TestDataBuilder.BuildCreateTaskRequest();

            _projectRepoMock.Setup(r => r.GetByIdAsync(request.ProjectId))
                            .ReturnsAsync((Project?)null);

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Project not found");

            // User repo should never even be called if project check fails
            _userRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenUserNotFound_ReturnsFailure()
        {
            // Arrange
            var project = TestDataBuilder.BuildProject();
            var request = TestDataBuilder.BuildCreateTaskRequest(projectId: project.Id);

            _projectRepoMock.Setup(r => r.GetByIdAsync(request.ProjectId))
                            .ReturnsAsync(project);
            _userRepoMock.Setup(r => r.GetByIdAsync(request.AssignedToUserId))
                         .ReturnsAsync((User?)null);

            // Act
            var result = await _sut.CreateAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Assigned user not found");
        }

        // ── UpdateStatusAsync ────────────────────────────────────────────

        [Fact]
        public async Task UpdateStatusAsync_WithValidStatus_ReturnsSuccess()
        {
            // Arrange
            var task = TestDataBuilder.BuildTask();
            var request = new UpdateTaskStatusRequest { Status = "InProgress" };

            _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id))
                         .ReturnsAsync(task);
            _taskRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>()))
                         .Returns(Task.CompletedTask);          // ← mock UpdateAsync too
            _uowMock.Setup(u => u.SaveChangesAsync(default))
                    .ReturnsAsync(1);

            // Act
            var result = await _sut.UpdateStatusAsync(task.Id, request);

            // Assert
            result.IsSuccess.Should().BeTrue(
                because: $"status '{request.Status}' is valid and task exists. Error: {result.Error}");
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_WithInvalidStatus_ReturnsFailure()
        {
            // Arrange
            var task = TestDataBuilder.BuildTask();
            var request = new UpdateTaskStatusRequest { Status = "Flying" };

            // Act
            var result = await _sut.UpdateStatusAsync(task.Id, request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Status must be");
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task UpdateStatusAsync_WhenTaskNotFound_ReturnsFailure()
        {
            // Arrange — use a valid status so validation passes, only task lookup fails
            var nonExistentId = Guid.NewGuid();
            var request = new UpdateTaskStatusRequest { Status = "Done" };

            _taskRepoMock.Setup(r => r.GetByIdAsync(nonExistentId))
                         .ReturnsAsync((TaskItem?)null);        // ← task doesn't exist

            // Act
            var result = await _sut.UpdateStatusAsync(nonExistentId, request);

            // Assert — failure should be about task not found, not validation
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain(nonExistentId.ToString());
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }

        // ── DeleteAsync ──────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_WhenTaskExists_ReturnsSuccess()
        {
            // Arrange
            var task = TestDataBuilder.BuildTask();
            _taskRepoMock.Setup(r => r.GetByIdAsync(task.Id))
                         .ReturnsAsync(task);
            _uowMock.Setup(u => u.SaveChangesAsync(default))
                    .ReturnsAsync(1);

            // Act
            var result = await _sut.DeleteAsync(task.Id);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _taskRepoMock.Verify(r => r.DeleteAsync(task.Id), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenTaskNotFound_ReturnsFailure()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _taskRepoMock.Setup(r => r.GetByIdAsync(nonExistentId))
                         .ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _sut.DeleteAsync(nonExistentId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            _taskRepoMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
        }
    }
}
