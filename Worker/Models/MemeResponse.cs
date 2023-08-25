namespace Worker.Models
{
    public class MemeResponse
    {
        public bool nsfw { get; set; }
        public string url { get; set; }

        public MemeResponse(bool nsfw, string url)
        {
            this.nsfw = nsfw;
            this.url = url;
        }
    }
}
