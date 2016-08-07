using System.Configuration;

namespace PluralsightDownloader.Web
{
    public static class Constants
    {
        public static readonly string BASE_URL = ConfigurationManager.AppSettings["BASE_URL"];
        public static readonly string LOGIN_URL = ConfigurationManager.AppSettings["LOGIN_URL"];
        public static readonly string COURSE_DATA_URL = ConfigurationManager.AppSettings["COURSE_DATA_URL"];
        public static readonly string COURSE_CONTENT_DATA_URL = ConfigurationManager.AppSettings["COURSE_CONTENT_DATA_URL"];
        public static readonly string COURSE_CLIP_DATA_URL = ConfigurationManager.AppSettings["COURSE_CLIP_DATA_URL"];
        public static readonly string COURSE_EXERCISE_FILES_URL = ConfigurationManager.AppSettings["COURSE_EXERCISE_FILES_URL"];
        public static readonly string COURSE_RETRIEVE_URL = ConfigurationManager.AppSettings["COURSE_RETRIEVE_URL"];
        public static readonly string USER_NAME = ConfigurationManager.AppSettings["USER_NAME"];
        public static readonly string PASSWORD = ConfigurationManager.AppSettings["PASSWORD"];
        public static readonly string AUTH_COOKIE = "AuthCookie";
        public static readonly string DOWNLOAD_FOLDER_PATH = ConfigurationManager.AppSettings["DOWNLOAD_FOLDER_PATH"] + "Pluralsight Downloader";
        public static readonly int CLIP_DOWNLOAD_SPEED_MULTIPLIER = int.Parse(ConfigurationManager.AppSettings["CLIP_DOWNLOAD_SPEED_MULTIPLIER"]);
    }
}