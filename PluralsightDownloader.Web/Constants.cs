using System.Configuration;

namespace PluralsightDownloader.Web
{
    public static class Constants
    {
        public static readonly string BASE_URL = ConfigurationManager.AppSettings["BASE_URL"];
        public static readonly string LOGIN_URL = ConfigurationManager.AppSettings["LOGIN_URL"];
        public static readonly string COURSE_DATA_URL = ConfigurationManager.AppSettings["COURSE_DATA_URL"];
        public static readonly string COURSE_CONTENT_DATA_URL = ConfigurationManager.AppSettings["COURSE_CONTENT_DATA_URL"];
        public static readonly string USER_NAME = ConfigurationManager.AppSettings["USER_NAME"];
        public static readonly string PASSWORD = ConfigurationManager.AppSettings["PASSWORD"];
        public static readonly string AUTH_COOKIE = "AuthCookie";
        public static readonly string DOWNLOAD_FOLDER_PATH = ConfigurationManager.AppSettings["DOWNLOAD_FOLDER_PATH"] + "Pluralsight Downloader";
    }
}