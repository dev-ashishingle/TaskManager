using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Controllers
{
    public class TasksController : BaseController
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>Get all tasks</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _taskService.GetAllAsync();
            return OkOrNotFound(result);
        }

        /// <summary>Get a single task by ID</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _taskService.GetByIdAsync(id);
            return OkOrNotFound(result);
        }

        /// <summary>Get all tasks assigned to a user</summary>
        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var result = await _taskService.GetByUserIdAsync(userId);
            return OkOrNotFound(result);
        }

        /// <summary>Create a new task</summary>
        [HttpPost]
        [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
        {
            var result = await _taskService.CreateAsync(request);
            return CreatedOrBadRequest(result, nameof(GetById), new { id = result.Value?.Id });
        }

        /// <summary>Update the status of a task</summary>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
        {
            var result = await _taskService.UpdateStatusAsync(id, request);
            return NoContentOrBadRequest(result);
        }

        /// <summary>Delete a task</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _taskService.DeleteAsync(id);
            return NoContentOrNotFound(result);
        }
    }
}
