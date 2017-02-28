using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class Course
    {
        public string Title { get; set; }

        public string Level { get; set; }

        public string Duration { get; set; }

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

        public long DurationSeconds
        {
            get
            {
                return Course.ConvertDurationToSeconds(Duration);
            }
        }

        public static long ConvertDurationToSeconds(string duration)
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
            }
            else
            {
                string[] times = duration.Split(':');
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