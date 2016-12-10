using System;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CoursePayload
    {
        public string Title { get; set; }

        public string Name { get; set; }        

        public bool CourseHasCaptions { get; set; }

        public bool SupportsWideScreenVideoFormats { get; set; }

        public TranslationLanguage[] TranslationLanguages { get; set; }

        public DateTime Timestamp { get; set; }
    }
}