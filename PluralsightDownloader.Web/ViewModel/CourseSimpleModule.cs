using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseSimpleModule
    {
        private static int GlobalIndex = 0;

        public int ModuleIndex { get; set; }

        public string ID { get; set; }

        public string Title { get; set; }

        public string PlayerUrl { get; set; }

        public string Duration { get; set; }

        public List<CourseSimpleClip> Clips { get; set; }

        public CourseSimpleModule()
        {
            this.ModuleIndex = GlobalIndex++;
            CourseSimpleClip.ResetIndex();
        }
    }
}