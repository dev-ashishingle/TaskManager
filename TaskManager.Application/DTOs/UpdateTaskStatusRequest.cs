using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Application.DTOs
{
    public class UpdateTaskStatusRequest
    {
        public string Status { get; init; } = string.Empty;
    }
}
