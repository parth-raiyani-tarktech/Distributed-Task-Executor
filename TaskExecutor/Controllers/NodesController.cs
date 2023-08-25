using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskExecutor.Models;

namespace TaskExecutor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NodesController : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public IActionResult RegisterNode([FromBody] NodeRegistrationRequest node)
        {
            // TODO: Implement this method

            return Ok();
        }
        
        [HttpDelete]
        [Route("unregister/{name}")]
        public IActionResult RegisterNode(string name)
        {
            throw new NotImplementedException();
        }
    }
}
