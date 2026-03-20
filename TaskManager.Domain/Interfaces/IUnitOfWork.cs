using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Domain.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        ITaskRepository Tasks { get; }
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
