using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskExecutor.Models;
using TaskExecutor.Services;

namespace TaskExecutor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly TaskAllocator _taskAllocator;
        public NodesController()
        {
            _taskAllocator = new TaskAllocator();
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterNode([FromBody] NodeRegistrationRequest node)
        {
            // TODO: Implement this method
            _taskAllocator.RegisterNode(node.Name, node.Address);
            return Ok();
        }
        
        [HttpDelete]
        [Route("unregister/{name}")]
        public IActionResult UnRegisterNode(string name)
        {
            _taskAllocator.UnregisterNode(name);
            return Ok();
        }

        [HttpGet]
        [Route("scheduletask")]
        public IActionResult ScheduleTask()
        {
            _taskAllocator.ScheduleTask();
            return Ok();
        }

        [HttpGet]
        [Route("getNodes")]
        public IActionResult GetNodes()
        {
            return (IActionResult)_taskAllocator.GetNodes();
        }
    }
}
