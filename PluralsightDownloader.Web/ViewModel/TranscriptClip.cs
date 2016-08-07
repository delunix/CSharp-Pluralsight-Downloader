namespace PluralsightDownloader.Web.ViewModel
{
    public class TranscriptClip
    {
        public string Title { get; set; }
        public string PlayerUrl { get; set; }
        public TranscriptSegment[] Segments { get; set; }
    }
}