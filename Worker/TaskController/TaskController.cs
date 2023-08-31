using System;
using System.IO;
using Worker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Collections;

namespace Worker.TaskController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private const string URL = "https://meme-api.com/gimme/wholesomememes";
        private readonly WorkerInfo _worker;
        public TaskController(WorkerInfo worker)
        {
            _worker = worker;
        }


        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> SaveMeme()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            HttpClient client = new HttpClient(clientHandler);
            //MemeResponse? memeResponse = await client.GetFromJsonAsync<MemeResponse>(URL);

            /*MemoryStream memoryStream;
            if (memeResponse != null && !memeResponse.nsfw)
            {
                byte[] bytes = await client.GetByteArrayAsync(memeResponse.url);
                memoryStream = new MemoryStream(bytes);

                string fileName = "image-" + Guid.NewGuid() +".jpg";
                FileStream fs = System.IO.File.Create(_worker.WorkDir + fileName, bytes.Length);
                fs.Write(bytes, 0, bytes.Length);
            }*/

           

            return Ok("Done");//Here, return the taskStatus
        }
    }
}