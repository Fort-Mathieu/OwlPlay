namespace OwlSongs
{
    internal class MediaItem
    {
        private string media;

        public MediaItem(string media)
        {
            this.media = media;
        }
        public string Media
        {
            get { return media; }
            set { media = value; }
        }
    }
}