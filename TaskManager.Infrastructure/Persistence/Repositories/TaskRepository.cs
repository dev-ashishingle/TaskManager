using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem?> GetByIdAsync(Guid id)
            => await _context.Tasks
                             .Include(t => t.Project)
                             .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
            => await _context.Tasks
                             .Include(t => t.Project)
                             .AsNoTracking()   // ← performance: read-only, skip change tracking
                             .ToListAsync();

        public async Task<IEnumerable<TaskItem>> GetByUserIdAsync(Guid userId)
            => await _context.Tasks
                             .Where(t => t.AssignedToUserId == userId)
                             .AsNoTracking()
                             .ToListAsync();

        public async Task AddAsync(TaskItem task)
            => await _context.Tasks.AddAsync(task);

        public Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task is not null)
                _context.Tasks.Remove(task);
        }
    }
}
