using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class Course
    {
        public string Title { get; set; }

        public string Level { get; set; }

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

        public string ReleaseDate { get; set; }

        public string Name { get; set; }

        public List<Author> Authors { get; set; }

        public bool HasTranscript { get; set; }

        public CourseRating CourseRating { get; set; }

        public bool IsRetired { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public bool IsBookmarked { get; set; }

        public bool UserMaySaveCourse { get; set; }

        public string ReplacementCourseName { get; set; }

        public string RetiredReason { get; set; }

        public string ReplacementCourseTitle { get; set; }

        public bool IsValid { get; set; }

        public bool IsUserAuthorizedForTranscript { get; set; }

        public bool SupportsWideScreenVideoFormats { get; set; }

        public CourseContent Content { get; set; }

        public ExerciseFiles ExerciseFiles { get; set; }

        public static string FormatDuration(string duration)
        {
            long hour = 0;
            long minute = 0;
            long second = 0;
            if (duration.StartsWith("PT"))
            {
                string sTimes = duration.Substring(2);
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
                duration = hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2");
            }
            return duration;
        }
    }
}