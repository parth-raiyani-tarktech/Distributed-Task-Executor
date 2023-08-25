using System.Net;
using System.Drawing;
using System.Text.Json;
using Worker.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Worker.TaskController
{
    public class TaskController
    {
        private const string URL = "https://meme-api.com/gimme/wholesomememes";
        public void SaveMeme()
        {
            WebRequest request = HttpWebRequest.Create(URL);
            WebResponse response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string urlText = reader.ReadToEnd();

            MemeResponse memeResponse = JsonSerializer.Deserialize<MemeResponse>(urlText);

            if(memeResponse != null && !memeResponse.nsfw)
            {
                WebClient wc = new WebClient();
                byte[] bytes = wc.DownloadData(memeResponse.url);
                MemoryStream memoryStream = new MemoryStream(bytes);
                //Convert and save Image
            }
        }
    }
}
