using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.DTOs
{
    public class TaskDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string Status { get; init; } = string.Empty;
        public string Priority { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? DueDate { get; init; }
        public Guid AssignedToUserId { get; init; }
        public Guid ProjectId { get; init; }
        public string ProjectName { get; init; } = string.Empty;
    }
}
