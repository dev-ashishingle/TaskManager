using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.DTOs
{
    public record CreateTaskRequest
    {
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string Priority { get; init; } = string.Empty;
        public DateTime? DueDate { get; init; }
        public Guid AssignedToUserId { get; init; }
        public Guid ProjectId { get; init; }
    }
}
