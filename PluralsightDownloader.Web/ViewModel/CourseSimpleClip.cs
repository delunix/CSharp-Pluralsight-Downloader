using System.Collections.Generic;
using PluralsightDownloader.Web.Extensions;
using System.IO;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseSimpleClip
    {
        private static int GlobalIndex = 0;
        public static void ResetIndex()
        {
            GlobalIndex = 0;
        }

        public int ClipIndex { get; set; }

        public int ModuleIndex { get; set; }

        public string ID { get; set; }

        public string Title { get; set; }

        public string PlayerUrl { get; set; }

        private string _duration = null;
        public string Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = Course.FormatDuration(value);
            }
        }

        public List<object> Transcripts { get; set; }

        public TranscriptClip TranscriptClip { get; set; }

        public string VideoDirectory { get; set; }

        public string FileName
        {
            get
            {
                return ((this.ClipIndex + 1).ToString("D2") + " - " + this.Title + ".mp4").ToValidFileName();
            }
        }

        public bool HasBeenViewed { get; set; }

        public bool HasBeenDownloaded
        {
            get
            {
                return File.Exists(this.VideoDirectory + "//" + this.FileName);
            }
            set
            {

            }
        }

        public string PlayerParameters { get; set; }

        public bool UserMayViewClip { get; set; }

        public string ClickActionDescription { get; set; }

        public bool IsHighlighted { get; set; }

        public string Name { get; set; }

        public bool IsBookmarked { get; set; }

        public string HasBeenViewedImageUrl { get; set; }

        public string HasBeenViewedAltText { get; set; }

        public ProgressArgs Progress { get; set; }

        public CourseSimpleClip()
        {
            this.ClipIndex = GlobalIndex++;
            Progress = new ProgressArgs() { TotalBytes = 1 };
        }

        public long DurationSeconds
        {
            get
            {
                long hour = 0;
                long minute = 0;
                long second = 0;
                string[] times = _duration.Split(':');
                hour = long.Parse(times[0]);
                minute = long.Parse(times[1]);
                second = long.Parse(times[2]);
                return (hour * 3600
                            + minute * 60
                            + second);
            }
        }
    }
}