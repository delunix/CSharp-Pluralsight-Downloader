namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseRating
    {
        public int CurrentUsersRating { get; set; }

        public double AverageRating { get; set; }

        public double Rating { get; set; }

        public bool CanRateThisCourse { get; set; }

        public string CourseName { get; set; }

        public int NumberOfRaters { get; set; }

        public bool HasUserRatedCourse { get; set; }
    }
}