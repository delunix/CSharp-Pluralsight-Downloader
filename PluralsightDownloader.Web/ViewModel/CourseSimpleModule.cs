using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseSimpleModule
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string PlayerUrl { get; set; }

        public string Duration { get; set; }

        public List<CourseClip> Clips { get; set; }
    }
}