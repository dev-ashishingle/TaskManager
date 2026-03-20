using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Navigation property — EF Core will populate this
        public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

        private Project() { }

        public static Project Create(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Project name cannot be empty.");

            return new Project
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
