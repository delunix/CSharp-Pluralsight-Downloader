namespace PluralsightDownloader.Web.ViewModel
{
    public class ClipToSave : CourseSimpleClip
    {
        public bool SupportsWideScreenVideoFormats { get; set; }

        public string CourseTitle { get; set; }

        public string ModuleTitle { get; set; }
    }
}