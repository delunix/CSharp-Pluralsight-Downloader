using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class CourseContent
    {
        public string ID { get; set; }

        public string PublishedOn { get; set; }

        public string UpdatedOn { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }
        
        public string Description { get; set; }

        public string Level { get; set; }
        
        private string _duration = null;
        public string Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = Course.FormatDuration(value);
            }
        }

        public string PopularityScore { get; set; }

        public bool HasTranscript { get; set; }

        public bool HasAssessment { get; set; }

        public bool HasLearningCheck { get; set; }

        public string MentorDepartment { get; set; }

        public string PlayerURL { get; set; }

        public List<CourseSkill> SkillPaths { get; set; }

        public CourseRetired Retired { get; set; }

        public CourseSimpleRating Rating { get; set; }

        public List<CourseSimpleModule> Modules { get; set; }

        public CourseImage CourseImage { get; set; }

        public List<CourseAuthor> Authors { get; set; }

        public List<string> Audience { get; set; }
    }
}