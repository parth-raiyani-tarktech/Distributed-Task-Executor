using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using TaskExecutor.Services;
using TaskExecutor.Services.Interface;
using Task = TaskExecutor.Models.Task;

namespace TaskExecutor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskManager _taskManager;
        private readonly ITaskOrchestrator _taskOrchestrator;
        public TaskController(ITaskOrchestrator taskAllocator, ITaskManager taskManager)
        {
            _taskOrchestrator = taskAllocator;
            _taskManager = taskManager;
        }

        [HttpPost]
        public IActionResult CreateTask()
        {
            var task = _taskManager.CreateTask();
            _taskOrchestrator.ExecuteTaskAsync();
            return Ok(task);
        }

        [HttpGet]
        public IActionResult GetTask(Models.TaskStatus status)
        {
            return Ok(_taskManager.GetTasksByStatus(status));
        }

        [HttpGet("all-tasks-in-execution-order")]
        public IActionResult GetAllTasksInExecutionOrder()
        {
            return Ok(_taskManager.GetAllTasksInExecutionOrder());
        }
    }
}
