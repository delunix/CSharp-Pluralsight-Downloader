using System;

namespace PluralsightDownloader.Web.ViewModel
{
    public class TranscriptSegment
    {
        public string Text { get; set; }
        public float DisplayTime { get; set; }

        public string DisplayTimeString
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(DisplayTime);
                return time.ToString(@"hh\:mm\:ss\,fff");
            }
        }

        public string GetOffsetDisplayTimeString(float offsetSeconds, float seconds = 0)
        {
            if (seconds == 0)
                seconds = DisplayTime;

            if (seconds >= offsetSeconds)
            {
                TimeSpan time = TimeSpan.FromSeconds(seconds - offsetSeconds);
                return time.ToString(@"hh\:mm\:ss\,fff");
            }
            return "00:00:00,000";
        }
    }
}