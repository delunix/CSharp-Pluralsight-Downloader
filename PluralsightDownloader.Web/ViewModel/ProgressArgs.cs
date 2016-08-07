namespace PluralsightDownloader.Web.ViewModel
{
    public class ProgressArgs
    {
        public string Id { get; set; }

        public long TotalBytes { get; set; }

        public long BytesReceived { get; set; }

        public float PercentComplete { get { return (float)BytesReceived / TotalBytes; } }

        public string FileName { get; set; }

        public bool IsFinished { get { return BytesReceived == TotalBytes; } }

        public bool IsDownloading { get; set; }

        public object Extra { get; set; }
    }
}