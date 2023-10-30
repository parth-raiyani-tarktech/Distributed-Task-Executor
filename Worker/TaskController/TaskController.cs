using Worker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Worker.TaskController
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private const string URL = "https://meme-api.com/gimme/wholesomememes";
        private readonly WorkerInfo _worker;
        private static readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public TaskController(WorkerInfo worker)
        {
            _worker = worker;
        }

        [HttpGet]
        [Route("health-check")]
        public IActionResult IsWorkerUp()
        {
            return Ok("I'm up!");
        }

        [HttpPost]
        [Route("abort")]
        public void AbortTask()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        [HttpGet]
        [Route("execute")]
        public async Task<IActionResult> SaveMemeAsync()
        {
            await Task.Run(async () => await SaveMemeAsJPGAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);


            return Ok(Models.TaskStatus.Completed);
        }

        private async Task SaveMemeAsJPGAsync(CancellationToken token)
        {
            HttpClient client = new HttpClient();
            MemeResponse? memeResponse = await client.GetFromJsonAsync<MemeResponse>(URL, token);
            
            if (memeResponse != null && !memeResponse.nsfw)
            {
                byte[] bytes = await client.GetByteArrayAsync(memeResponse.url, token);

                string fileName = "image-" + Guid.NewGuid() + ".jpg";
                if (!Directory.Exists(_worker.WorkDir))
                    Directory.CreateDirectory(_worker.WorkDir);

                FileStream fs = System.IO.File.Create(_worker.WorkDir + fileName, bytes.Length);
                await fs.WriteAsync(bytes, 0, bytes.Length, token);
            }
        }
    }
}