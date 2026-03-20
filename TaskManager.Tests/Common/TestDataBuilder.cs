using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests.Common
{
    public static class TestDataBuilder
    {
        public static Project BuildProject(string name = "Test Project")
            => Project.Create(name, "Test description");

        public static User BuildUser(string email = "test@example.com")
            => User.Create("Test User", email, "hashed_password");

        public static TaskItem BuildTask(Guid? projectId = null, Guid? userId = null)
            => TaskItem.Create(
                title: "Test Task",
                description: "Test description",
                priority: Priority.Medium,
                dueDate: DateTime.UtcNow.AddDays(7),
                assignedToUserId: userId ?? Guid.NewGuid(),
                projectId: projectId ?? Guid.NewGuid());

        public static CreateTaskRequest BuildCreateTaskRequest(
            Guid? projectId = null,
            Guid? userId = null) => new()
            {
                Title = "Test Task",
                Description = "Test description",
                Priority = "Medium",
                DueDate = DateTime.UtcNow.AddDays(7),
                AssignedToUserId = userId ?? Guid.NewGuid(),
                ProjectId = projectId ?? Guid.NewGuid()
            };
    }
}
