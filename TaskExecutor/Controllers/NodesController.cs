using Microsoft.AspNetCore.Mvc;
using TaskExecutor.Models;
using TaskExecutor.Services.Interface;

namespace TaskExecutor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        private readonly ITaskOrchestrator _taskOrchestrator;
        private readonly INodeManager _nodeManager;
        public NodesController(ITaskOrchestrator taskAllocator, INodeManager nodeManager)
        {
            _taskOrchestrator = taskAllocator;
            _nodeManager = nodeManager;
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterNode([FromBody] NodeRegistrationRequest node)
        {
            _nodeManager.RegisterNode(node.Name, node.Address);
            _taskOrchestrator.ExecuteTaskAsync();
            return Ok();
        }
        
        [HttpDelete]
        [Route("unregister/{name}")]
        public IActionResult UnRegisterNode(string name)
        {
            var node = _nodeManager?.GetNodeByName(name);

            _taskOrchestrator.StopOngoingTaskOf(node);

            _nodeManager?.UnregisterNode(node);
            _taskOrchestrator.ExecuteTaskAsync();
            return Ok();
        }

        [HttpGet]
        [Route("get-all-Nodes")]
        public IActionResult GetAllNodes()
        {
            return Ok(_nodeManager.GetAllNodes());
        }

        [HttpGet]
        [Route("shutting-down")]
        public void ShuttingDownNode(string nodeName)
        {
            Console.WriteLine($"{nodeName} is shutting down!");

            var node = _nodeManager?.GetNodeByName(nodeName);
            _nodeManager?.OnNodeDown(node);

            _taskOrchestrator.StopOngoingTaskOf(node);
            _taskOrchestrator.ExecuteTaskAsync();
        }
    }
}
