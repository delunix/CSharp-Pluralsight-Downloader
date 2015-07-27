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

        public List<CourseModule> CourseModules { get; set; }
    }
}