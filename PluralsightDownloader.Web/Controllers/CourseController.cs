using MediaToolkit.Model;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using NLog;
using PluralsightDownloader.Web.Extensions;
using PluralsightDownloader.Web.Hubs;
using PluralsightDownloader.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PluralsightDownloader.Web.Controllers
{
    [RoutePrefix("api/courses")]
    public class CourseController : ApiController
    {
        #region Properties

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion Properties

        #region Actions

        [HttpGet]
        [Route("{coursename}")]
        public IHttpActionResult CourseData(string coursename)
        {
            Course course = new Course();
            String json = String.Empty;
            try
            {
                using (var webClient = new WebClient())
                {
                    json = webClient.DownloadString(string.Format(Constants.COURSE_DATA_URL, coursename));
                    course = JsonConvert.DeserializeObject<Course>(json);

                    NameValueCollection postData = new NameValueCollection() { { "courseId", coursename } };
                    byte[] responsebytes = webClient.UploadValues(Constants.COURSE_PAYLOAD_DATA_URL, postData);
                    json = Encoding.UTF8.GetString(responsebytes);
                    var coursePayload = JsonConvert.DeserializeObject<CoursePayload>(json);
                    if (coursePayload == null)
                    {
                        logger.Error("Couldn't retrieve course. Requested course={0}", coursename);
                        throw new Exception("Couldn't retrieve course data. Please check log file.");
                    }
                    course.SupportsWideScreenVideoFormats = coursePayload.SupportsWideScreenVideoFormats;

                    json = webClient.DownloadString(string.Format(Constants.COURSE_CONTENT_DATA_URL, coursename));
                    course.Content = JsonConvert.DeserializeObject<CourseContent>(json);

                    SetupAuthenticationCookie(webClient);
                    // TODO: check if the user has access to exercise files.
                    //json = webClient.DownloadString(string.Format(Constants.COURSE_EXERCISE_FILES_URL, coursename));
                    //course.ExerciseFiles = JsonConvert.DeserializeObject<ExerciseFiles>(json);

                    json = webClient.DownloadString(string.Format(Constants.COURSE_TRANSCRIPT_URL, coursename));
                    var transcript = JsonConvert.DeserializeObject<Transcript>(json);

                    course.Content.Modules.ForEachWithIndex((module, moduleIndex) =>
                    {
                        module.Clips.ForEachWithIndex((clip, clipIndex) =>
                        {
                            clip.TranscriptClip = transcript.Modules[moduleIndex].Clips[clipIndex];
                        });
                    });
                }
            }
            catch (Exception exception)
            {
                return HandleException(exception);
            }

            return Ok(course);
        }

        [Route("clip/{clipname}/download/")]
        public async Task<IHttpActionResult> DownloadCourseModuleClip(string clipname, ClipToSave clipToSave)
        {
            ClipDownloadData clipUrl = null;
            // 1- get the video clip url to download.
            try
            {
                clipUrl = GetClipUrl(clipToSave);

                // 2- make sure the folders structure exist.
                var videoSaveDirectory = SetUpVideoFolderStructure(clipToSave.CourseTitle,
                    (clipToSave.ModuleIndex + 1).ToString("D2") + " - " + clipToSave.ModuleTitle,
                    (clipToSave.ClipIndex + 1).ToString("D2") + " - " + clipToSave.Title);

                // 3- download the video and report progress back.
                int receivedBytes = 0;
                long totalBytes = 0;
                var videoFileName = ((clipToSave.ClipIndex + 1).ToString("D2") + " - " + clipToSave.Title + ".mp4").ToValidFileName();
                var videoSaveLocation = videoSaveDirectory.FullName + "\\raw-" + videoFileName;

                using (var client = new WebClient())
                using (var regStream = await client.OpenReadTaskAsync(clipUrl.URLs[0].URL))
                using (var stream = new ThrottledStream(regStream, 115200))
                {
                    byte[] buffer = new byte[1024];
                    totalBytes = Int32.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
                    stream.MaximumBytesPerSecond = GetClipMaxDownloadSpeed(clipToSave.DurationSeconds, totalBytes);

                    using (var fileStream = File.OpenWrite(videoSaveLocation))
                    {
                        for (;;)
                        {
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                            {
                                await Task.Yield();
                                break;
                            }

                            receivedBytes += bytesRead;
                            var progress = new ProgressArgs()
                            {
                                Id = clipToSave.Name,
                                BytesReceived = receivedBytes,
                                FileName = videoFileName,
                                TotalBytes = totalBytes,
                                IsDownloading = true,
                                Extra = new { clipToSave.ModuleIndex, clipToSave.ClipIndex }
                            };
                            fileStream.Write(buffer, 0, bytesRead);
                            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
                            hubContext.Clients.All.updateProgress(progress);
                        }
                    }
                }

                // 4- save the video file.
                var inputFile = new MediaFile { Filename = videoSaveDirectory.FullName + "\\raw-" + videoFileName };
                var outputFile = new MediaFile { Filename = videoSaveDirectory.FullName + "\\" + videoFileName };

                if (File.Exists(outputFile.Filename))
                    File.Delete(outputFile.Filename);
                File.Move(inputFile.Filename, outputFile.Filename);

                // 5- Create srt files
                if (Constants.SUBTITLES)
                {
                    var srtFilename = outputFile.Filename.Substring(0, outputFile.Filename.Length - 4) + ".srt";
                    var srtString = clipToSave.TranscriptClip.GetSrtString(clipToSave.DurationSeconds);
                    File.WriteAllText(srtFilename, srtString);
                }

                return Ok(new ProgressArgs()
                {
                    Id = clipToSave.Name,
                    BytesReceived = receivedBytes,
                    FileName = videoFileName,
                    TotalBytes = totalBytes,
                    IsDownloading = false,
                    Extra = new { clipToSave.ModuleIndex, clipToSave.ClipIndex }
                });
            }
            catch (Exception exception)
            {
                return HandleException(exception);
            }
        }

        #endregion Actions

        #region Helpers

        private long GetClipMaxDownloadSpeed(long seconds, long totalBytes)
        {
            var maxSpeed = totalBytes / seconds;
            return maxSpeed * Constants.CLIP_DOWNLOAD_SPEED_MULTIPLIER;
        }

        // ToDo: videos location should be configurable from client.
        private DirectoryInfo SetUpVideoFolderStructure(string courseTitle, string moduleTitle, string clipTitle)
        {
            Directory.CreateDirectory(Constants.DOWNLOAD_FOLDER_PATH);
            Directory.CreateDirectory(Constants.DOWNLOAD_FOLDER_PATH + "\\PluralSight - " + courseTitle.ToValidFileName());

            return Directory.CreateDirectory(Constants.DOWNLOAD_FOLDER_PATH + "\\PluralSight - " +
                                          courseTitle.ToValidFileName() + "\\" + moduleTitle.ToValidFileName());
        }

        private ClipDownloadData GetClipUrl(ClipToSave clip)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(Constants.COURSE_CLIP_DATA_URL));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            http.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";

            var playerParameters = HttpUtility.ParseQueryString(clip.PlayerUrl.Split(new char[] { '?' }, 2)[1]);
            var playerParametersObj = new
            {
                author = playerParameters["author"],
                moduleName = playerParameters["name"],
                courseName = playerParameters["course"],
                clipIndex = int.Parse(playerParameters["clip"]),
                mediaType = "mp4",
                quality = (clip.SupportsWideScreenVideoFormats ? "1280x720" : "1024x768"),
                includeCaptions = false,
                locale = Constants.SUBTITLES_LOCALE
            };
            var encoding = new ASCIIEncoding();
            Byte[] dataBytes = encoding.GetBytes(JsonConvert.SerializeObject(playerParametersObj));

            using (Stream sendStream = http.GetRequestStream())
                sendStream.Write(dataBytes, 0, dataBytes.Length);

            // if the clip is not free, then the user must sign in first and set authentication cookie.
            if (!clip.UserMayViewClip)
                SetupAuthenticationCookie(http);
            try
            {
                using (var response = http.GetResponse())
                using (var receiveStream = response.GetResponseStream())
                using (var sr = new StreamReader(receiveStream))
                {
                    var clipurl = sr.ReadToEnd();
                    return JsonConvert.DeserializeObject<ClipDownloadData>(clipurl); ;
                }
            }
            catch
            {
                logger.Error("Couldn't retrieve clip URL. Request parameters={0}", JsonConvert.SerializeObject(playerParametersObj));
                throw new Exception("Couldn't retrieve clip URL. Please check log file.");
            }
        }

        private string LoginToPluralSight()
        {
            var req = (HttpWebRequest)WebRequest.Create(Constants.LOGIN_URL);
            req.AllowAutoRedirect = false;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            using (var writer = new StreamWriter(req.GetRequestStream()))
            {
                writer.Write("Username=" + HttpUtility.UrlEncode(Constants.USER_NAME) + "&Password=" + HttpUtility.UrlEncode(Constants.PASSWORD));
            }

            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.Headers.Add("Origin", Constants.BASE_URL);
            req.Headers.Add("Cache-Control", "max-age=0");

            using (var response = req.GetResponse())
            {
                var authCookie = response.Headers["Set-Cookie"];
                if (string.IsNullOrWhiteSpace(authCookie) || authCookie.Contains("signin-errors"))
                {
                    // ToDO: better handling of errors returned by pluralsight server.
                    logger.Error("Invalid user name or password.");
                    throw new UnauthorizedAccessException("Invalid user name or password.");
                }

                HttpContext.Current.Application[Constants.AUTH_COOKIE] = authCookie;
                return authCookie;
            }
        }

        private void SetupAuthenticationCookie(WebClient client)
        {
            if (HttpContext.Current.Application[Constants.AUTH_COOKIE] == null)
                client.Headers.Add("Cookie", LoginToPluralSight());
            else
                client.Headers.Add("Cookie", HttpContext.Current.Application[Constants.AUTH_COOKIE].ToString());
        }

        private void SetupAuthenticationCookie(WebRequest client)
        {
            if (HttpContext.Current.Application[Constants.AUTH_COOKIE] == null)
                client.Headers.Add("Cookie", LoginToPluralSight());
            else
                client.Headers.Add("Cookie", HttpContext.Current.Application[Constants.AUTH_COOKIE].ToString());
        }

        private IHttpActionResult HandleException(Exception exception)
        {
            logger.Trace(exception);
            if (exception is WebException)
            {
                var webException = (WebException)exception;
                if (webException.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = webException.Response as HttpWebResponse;
                    if (response != null)
                    {
                        switch ((int)response.StatusCode)
                        {
                            case 429:
                                // 429 means that that we are sending too many requests.
                                // So we need to wait a little before sending next request.
                                logger.Warn("Too many requests in a short time.");
                                return ResponseMessage(Request.CreateResponse((HttpStatusCode)429, "Too many requests in a short time. Please try again after some time."));
                        }
                    }
                }
            }

            return InternalServerError(exception);
        }

        #endregion Helpers
    }
}