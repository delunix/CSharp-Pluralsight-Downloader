namespace PluralsightDownloader.Web.ViewModel
{
    public class ClipToSave : CourseClip
    {
        public bool SupportsWideScreenVideoFormats { get; set; }

        public string CourseTitle { get; set; }

        public string ModuleTitle { get; set; }

        public int ModuleIndex { get; set; }
    }
}