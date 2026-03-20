using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.DTOs
{
    public class CreateProjectRequest
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
    }
}
