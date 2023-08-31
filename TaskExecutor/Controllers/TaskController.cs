using Microsoft.AspNetCore.Mvc;
using TaskExecutor.Services;
using Task = TaskExecutor.Models.Task;

namespace TaskExecutor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskAllocator _taskAllocator;
        public TaskController()
        {
            _taskAllocator = new TaskAllocator();
        }

        [HttpPost]
        public IActionResult CreateTask(Task task)
        {
            _taskAllocator.CreateTask(task);
            return Ok(task);
        }

        [HttpGet]
        public IActionResult GetTask(Models.TaskStatus status)
        {
            return Ok(_taskAllocator.GetTaskByStatus(status));
        }

        [HttpGet("all-tasks-in-execution-order")]
        public IActionResult GetAllTasksInExecutionOrder()
        {
            return Ok(_taskAllocator.GetAllTasksInExecutionOrder());
        }


        [HttpPost]
        [Route("schedule-tasks")]
        public IActionResult ScheduleTasks()
        {
            _taskAllocator.ScheduleTasks();
            return Ok();
        }
    }
}
