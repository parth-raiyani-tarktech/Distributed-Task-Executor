using Worker.Models;
using Microsoft.AspNetCore.Mvc;

namespace Worker.TaskController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private const string URL = "https://meme-api.com/gimme/wholesomememes";
        private readonly WorkerInfo _worker;
        private readonly int Timeout = 5;
        public TaskController(WorkerInfo worker)
        {
            _worker = worker;
        }

        [HttpGet]
        [Route("execute")]
        public IActionResult SaveMeme()
        {
            HttpClient client = new HttpClient();
            var response = Task.Run(async () => await SaveMemeAsJPGAsync());

            if (!response.Wait(TimeSpan.FromSeconds(Timeout)))
            {
                return Ok(Models.TaskStatus.Failed);
            }
            return Ok(response);
        }

        private async Task<Models.TaskStatus> SaveMemeAsJPGAsync()
        {
            HttpClient client = new HttpClient();
            MemeResponse? memeResponse = await client.GetFromJsonAsync<MemeResponse>(URL);
            
            if (memeResponse != null && !memeResponse.nsfw)
            {
                byte[] bytes = await client.GetByteArrayAsync(memeResponse.url);

                string fileName = "image-" + Guid.NewGuid() + ".jpg";
                FileStream fs = System.IO.File.Create(_worker.WorkDir + fileName, bytes.Length);
                fs.Write(bytes, 0, bytes.Length);
            }
            return Models.TaskStatus.Completed;
        }
    }
}