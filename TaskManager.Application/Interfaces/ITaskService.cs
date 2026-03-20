using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface ITaskService
    {
        Task<Result<IEnumerable<TaskDto>>> GetAllAsync();
        Task<Result<IEnumerable<TaskDto>>> GetByUserIdAsync(Guid userId);
        Task<Result<TaskDto>> GetByIdAsync(Guid id);
        Task<Result<TaskDto>> CreateAsync(CreateTaskRequest request);
        Task<Result> UpdateStatusAsync(Guid id, UpdateTaskStatusRequest request);
        Task<Result> DeleteAsync(Guid id);
    }
}
