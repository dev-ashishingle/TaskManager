using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateProjectRequest> _validator;

        public ProjectService(IUnitOfWork uow, IValidator<CreateProjectRequest> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<IEnumerable<ProjectDto>>> GetAllAsync()
        {
            var projects = await _uow.Projects.GetAllAsync();
            return Result<IEnumerable<ProjectDto>>.Success(projects.Select(MapToDto));
        }

        public async Task<Result<ProjectDto>> GetByIdAsync(Guid id)
        {
            var project = await _uow.Projects.GetByIdAsync(id);
            if (project is null)
                return Result<ProjectDto>.Failure($"Project with id {id} was not found.");

            return Result<ProjectDto>.Success(MapToDto(project));
        }

        public async Task<Result<ProjectDto>> CreateAsync(CreateProjectRequest request)
        {
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Result<ProjectDto>.Failure(
                    string.Join(", ", validation.Errors.Select(e => e.ErrorMessage)));

            var project = Project.Create(request.Name, request.Description);

            await _uow.Projects.AddAsync(project);
            await _uow.SaveChangesAsync();

            return Result<ProjectDto>.Success(MapToDto(project));
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var project = await _uow.Projects.GetByIdAsync(id);
            if (project is null)
                return Result.Failure($"Project with id {id} was not found.");

            await _uow.Projects.DeleteAsync(id);
            await _uow.SaveChangesAsync();

            return Result.Success();
        }

        private static ProjectDto MapToDto(Project project) => new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            TaskCount = project.Tasks.Count
        };
    }
}
