using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Persistence.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context) => _context = context;

        public async Task<Project?> GetByIdAsync(Guid id)
            => await _context.Projects
                             .Include(p => p.Tasks)
                             .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Project>> GetAllAsync()
            => await _context.Projects.AsNoTracking().ToListAsync();

        public async Task AddAsync(Project project)
            => await _context.Projects.AddAsync(project);

        public async Task DeleteAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project is not null)
                _context.Projects.Remove(project);
        }
    }
}
