using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Interfaces
{
    public interface IProjectService
    {
        Task<Result<IEnumerable<ProjectDto>>> GetAllAsync();
        Task<Result<ProjectDto>> GetByIdAsync(Guid id);
        Task<Result<ProjectDto>> CreateAsync(CreateProjectRequest request);
        Task<Result> DeleteAsync(Guid id);
    }
}
