using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Domain.Enums;
using TaskStatus = TaskManager.Domain.Enums.TaskStatus;

namespace TaskManager.Domain.Entities
{
    public class TaskItem
    {

        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public TaskStatus Status { get; private set; }
        public Priority Priority { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? DueDate { get; private set; }
        public Guid AssignedToUserId { get; private set; }
        public Guid ProjectId { get; private set; }
        public Project? Project { get; private set; }  // Navigation property

        // Private constructor forces use of the factory method below
        private TaskItem() { }

        // SOLID — Single Responsibility: entity controls its own creation rules
        public static TaskItem Create(string title, string? description,
                                       Priority priority, DateTime? dueDate,
                                       Guid assignedToUserId, Guid projectId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.");

            return new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Status = TaskStatus.Todo,
                Priority = priority,
                DueDate = dueDate,
                AssignedToUserId = assignedToUserId,
                CreatedAt = DateTime.UtcNow,
                ProjectId = projectId
            };
        }

        public void UpdateStatus(TaskStatus newStatus)
        {
            // Business rule: can't reopen a cancelled task
            if (Status == TaskStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a cancelled task.");

            Status = newStatus;
        }
    }
}
