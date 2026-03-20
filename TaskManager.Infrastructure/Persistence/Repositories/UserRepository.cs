using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User?> GetByIdAsync(Guid id)
            => await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users
                             .AsNoTracking()
                             .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

        public async Task AddAsync(User user)
            => await _context.Users.AddAsync(user);
    }
}
