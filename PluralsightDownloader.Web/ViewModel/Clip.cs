using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class Clip
    {
        public Clip()
        {
            Progress = new ProgressArgs() { TotalBytes = 1 };
        }

        public List<object> Transcripts { get; set; }

        public int ClipIndex { get; set; }

        public string Title { get; set; }

        public bool HasBeenViewed { get; set; }

        public string Duration { get; set; }

        public string PlayerParameters { get; set; }

        public bool UserMayViewClip { get; set; }

        public string ClickActionDescription { get; set; }

        public bool IsHighlighted { get; set; }

        public string Name { get; set; }

        public bool IsBookmarked { get; set; }

        public string HasBeenViewedImageUrl { get; set; }

        public string HasBeenViewedAltText { get; set; }

        public ProgressArgs Progress { get; set; }
    }
}