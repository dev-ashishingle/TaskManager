using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateTaskRequest> _createValidator;
        private readonly IValidator<UpdateTaskStatusRequest> _updateValidator;

        public TaskService(IUnitOfWork uow,
                           IValidator<CreateTaskRequest> createValidator,
                           IValidator<UpdateTaskStatusRequest> updateValidator)
        {
            _uow = uow;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<Result<IEnumerable<TaskDto>>> GetAllAsync()
        {
            var tasks = await _uow.Tasks.GetAllAsync();
            return Result<IEnumerable<TaskDto>>.Success(tasks.Select(MapToDto));
        }

        public async Task<Result<IEnumerable<TaskDto>>> GetByUserIdAsync(Guid userId)
        {
            var tasks = await _uow.Tasks.GetByUserIdAsync(userId);
            return Result<IEnumerable<TaskDto>>.Success(tasks.Select(MapToDto));
        }

        public async Task<Result<TaskDto>> GetByIdAsync(Guid id)
        {
            var task = await _uow.Tasks.GetByIdAsync(id);

            if (task is null)
                return Result<TaskDto>.Failure($"Task with id {id} was not found.");

            return Result<TaskDto>.Success(MapToDto(task));
        }

        public async Task<Result<TaskDto>> CreateAsync(CreateTaskRequest request)
        {
            // Step 1: validate input
            var validation = await _createValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return Result<TaskDto>.Failure(
                    string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));

            // Step 2: check project exists
            var project = await _uow.Projects.GetByIdAsync(request.ProjectId);
            if (project is null)
                return Result<TaskDto>.Failure("Project not found.");

            // Step 3: check user exists
            var user = await _uow.Users.GetByIdAsync(request.AssignedToUserId);
            if (user is null)
                return Result<TaskDto>.Failure("Assigned user not found.");

            // Step 4: parse enum (validator already confirmed it's valid)
            var priority = Enum.Parse<Priority>(request.Priority, ignoreCase: true);

            // Step 5: use the domain factory — entity owns creation rules
            var task = TaskItem.Create(
                request.Title,
                request.Description,
                priority,
                request.DueDate,
                request.AssignedToUserId,
                request.ProjectId);

            // Step 6: persist via UoW
            await _uow.Tasks.AddAsync(task);
            await _uow.SaveChangesAsync();

            return Result<TaskDto>.Success(MapToDto(task));
        }

        public async Task<Result> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request)
        {
            // Step 1: validate input
            var validation = await _updateValidator.ValidateAsync(request);
            if (!validation.IsValid)
                return Result.Failure(
                    string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));

            // Step 2: load entity
            var task = await _uow.Tasks.GetByIdAsync(id);
            if (task is null)
                return Result.Failure($"Task with id {id} was not found.");

            // Step 3: call domain method — business rule enforced inside the entity
            var newStatus = Enum.Parse<TaskStatus>(request.Status, ignoreCase: true);
            task.UpdateStatus(newStatus);   // throws if cancelled — entity owns the rule

            // Step 4: persist
            await _uow.Tasks.UpdateAsync(task);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var task = await _uow.Tasks.GetByIdAsync(id);
            if (task is null)
                return Result.Failure($"Task with id {id} was not found.");

            await _uow.Tasks.DeleteAsync(id);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        // Private mapper — keeps mapping logic in one place
        private static TaskDto MapToDto(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            AssignedToUserId = task.AssignedToUserId,
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.Name ?? string.Empty
        };
    }
}
