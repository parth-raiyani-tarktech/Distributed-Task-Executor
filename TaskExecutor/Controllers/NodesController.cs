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
        private readonly NodeManager _nodeManager;
        public NodesController(IServiceProvider serviceProvider)
        {
            _nodeManager = new NodeManager(serviceProvider);
        }

        [HttpPost]
        [Route("register")]
        public IActionResult RegisterNode([FromBody] NodeRegistrationRequest node)
        {
            _nodeManager.RegisterNode(node.Name, node.Address);
            return Ok();
        }
        
        [HttpDelete]
        [Route("unregister/{name}")]
        public IActionResult UnRegisterNode(string name)
        {
            NodeManager.UnregisterNode(name);
            return Ok();
        }

        [HttpGet]
        [Route("get-all-Nodes")]
        public IActionResult GetAllNodes()
        {
            return Ok(_nodeManager.GetAllNodes());
        }
    }
}
