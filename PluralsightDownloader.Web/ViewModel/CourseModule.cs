using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseModule
    {
        public bool UserMayViewFirstClip { get; set; }

        public string ModuleRef { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Duration { get; set; }

        public bool HasBeenViewed { get; set; }

        public bool IsHighlighted { get; set; }

        public string FragmentIdentifier { get; set; }

        public string FirstClipLaunchClickHandler { get; set; }

        public bool UserMayBookmark { get; set; }

        public bool IsBookmarked { get; set; }

        public List<Clip> Clips { get; set; }

        public string HasBeenViewedImageUrl { get; set; }

        public string HasBeenViewedAltText { get; set; }
    }
}