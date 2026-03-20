using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public DateTime CreatedAt { get; private set; }

        private User() { }

        public static User Create(string fullName, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.");

            return new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email.ToLowerInvariant(),
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
