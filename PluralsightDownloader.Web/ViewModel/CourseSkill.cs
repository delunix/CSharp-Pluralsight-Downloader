using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseSkill
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public bool Retired { get; set; }

        public string URL { get; set; }

        public string URLSlug { get; set; }

        public string IconURL { get; set; }
    }
}