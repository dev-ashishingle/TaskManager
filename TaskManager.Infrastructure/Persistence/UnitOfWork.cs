using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ITaskRepository Tasks { get; }
        public IUserRepository Users { get; }
        public IProjectRepository Projects { get; }

        public UnitOfWork(AppDbContext context,
                          ITaskRepository tasks,
                          IUserRepository users,
                          IProjectRepository projects)
        {
            _context = context;
            Tasks = tasks;
            Users = users;
            Projects = projects;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public async ValueTask DisposeAsync()
            => await _context.DisposeAsync();
    }
}
