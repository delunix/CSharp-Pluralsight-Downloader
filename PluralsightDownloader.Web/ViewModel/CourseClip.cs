using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseClip
    {
        public CourseClip()
        {
            Progress = new ProgressArgs() { TotalBytes = 1 };
        }

        public string ID { get; set; }

        public string Title { get; set; }

        public string PlayerUrl { get; set; }

        public string Duration { get; set; }

        public List<object> Transcripts { get; set; }

        public TranscriptClip TranscriptClip { get; set; }

        public int ClipIndex { get; set; }

        public bool HasBeenViewed { get; set; }

        public string PlayerParameters { get; set; }

        public bool UserMayViewClip { get; set; }

        public string ClickActionDescription { get; set; }

        public bool IsHighlighted { get; set; }

        public string Name { get; set; }

        public bool IsBookmarked { get; set; }

        public string HasBeenViewedImageUrl { get; set; }

        public string HasBeenViewedAltText { get; set; }

        public ProgressArgs Progress { get; set; }

        public long DurationSeconds
        {
            get
            {
                long hour = 0;
                long minute = 0;
                long second = 0;
                if (Duration.StartsWith("PT"))
                {
                    string sTimes = Duration.Substring(2);
                    if (sTimes.Contains("H"))
                    {
                        long.TryParse(sTimes.Split('H')[0].Split('.')[0], out hour);
                        sTimes = sTimes.Split('H')[1];
                    }
                    if (sTimes.Contains("M"))
                    {
                        long.TryParse(sTimes.Split('M')[0].Split('.')[0], out minute);
                        sTimes = sTimes.Split('M')[1];
                    }
                    if (sTimes.Contains("S"))
                    {
                        long.TryParse(sTimes.Split('S')[0].Split('.')[0], out second);
                        sTimes = sTimes.Split('S')[1];
                    }
                }
                else
                {
                    string[] times = Duration.Split(':');
                    hour = long.Parse(times[0]);
                    minute = long.Parse(times[1]);
                    second = long.Parse(times[2]);
                }
                return (hour * 3600
                            + minute * 60
                            + second);
            }
        }
    }
}