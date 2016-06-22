using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using PluralsightDownloader.Web.Extensions;
using PluralsightDownloader.Web.Hubs;
using PluralsightDownloader.Web.ViewModel;
using System;
using System.Collections.Generic;
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
        [HttpGet]
        [Route("{coursename}")]
        public IHttpActionResult CourseData(string coursename)
        {
            Course course;
            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString(string.Format(Constants.COURSE_DATA_URL, coursename));
                course = JsonConvert.DeserializeObject<Course>(json);
            }

            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadString(string.Format(Constants.COURSE_CONTENT_DATA_URL, coursename));
                course.CourseModules = JsonConvert.DeserializeObject<List<CourseModule>>(json);
            }

            return Ok(course);
        }

        [Route("clip/{clipname}/download/")]
        public async Task<IHttpActionResult> DownloadCourseModuleClip(string clipname, ClipToSave clipToSave)
        {
            string clipUrl = string.Empty;
            // 1- get the video clip url to download.
            try
            {
                clipUrl = GetClipUrl(clipToSave);
            }
            catch (WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = webException.Response as HttpWebResponse;
                    if (response != null)
                    {
                        switch ((int)response.StatusCode)
                        {
                            // if the request is unauthorized or bad, try to set up authentication cookie again..
                            case 401:
                            case 400:
                                SetupAuthenticationCookie();
                                clipUrl = GetClipUrl(clipToSave);
                                break;

                            case 429:
                                // 429 means that that we are sending too many requests.
                                // So we need to wait a little before sending next request.
                                return ResponseMessage(Request.CreateResponse((HttpStatusCode)429,
                                    "Too many requests in a short time. Please try again after some time."));
                        }
                    }
                }
            }
            catch (HttpResponseException responseException)
            {
                switch ((int)responseException.Response.StatusCode)
                {
                    case 422:
                        // 422 means Invalid user name or password.
                        return
                            ResponseMessage(Request.CreateResponse((HttpStatusCode)422, "Invalid user name or password."));
                }
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }

            // 2- make sure the folders structure exist.
            var videoSaveDirectory = SetUpVideoFolderStructure(clipToSave.CourseTitle,
                (clipToSave.ModuleIndex + 1) + "- " + clipToSave.ModuleTitle,
                (clipToSave.ClipIndex + 1) + "- " + clipToSave.Title);

            // 3- download the video and report progress back.
            int receivedBytes = 0;
            int totalBytes = 0;
            var videoFileName = ((clipToSave.ClipIndex + 1) + "- " + clipToSave.Title + ".mp4").ToValidFileName();
            var videoSaveLocation = videoSaveDirectory.FullName + "\\" + videoFileName;
            var client = new WebClient();

            using (var stream = await client.OpenReadTaskAsync(clipUrl))
            {
                byte[] buffer = new byte[8192];
                totalBytes = Int32.Parse(client.ResponseHeaders[HttpResponseHeader.ContentLength]);
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

        // ToDo: videos location should be configurable from client.
        private DirectoryInfo SetUpVideoFolderStructure(string courseTitle, string moduleTitle, string clipTitle)
        {
            Directory.CreateDirectory(Constants.DOWNLOAD_FOLDER_PATH);
            Directory.CreateDirectory(Constants.DOWNLOAD_FOLDER_PATH + "\\" + courseTitle.ToValidFileName());
            return Directory.CreateDirectory(Constants.DOWNLOAD_FOLDER_PATH + "\\" + courseTitle.ToValidFileName() + "\\" + moduleTitle.ToValidFileName());
        }

        private string GetClipUrl(ClipToSave clip)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(Constants.COURSE_CLIP_DATA_URL));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            http.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";

            var playerParameters = HttpUtility.ParseQueryString(clip.PlayerParameters);
            var playerParametersObj = new
            {
                a = playerParameters["author"],
                m = playerParameters["name"],
                course = playerParameters["course"],
                cn = playerParameters["clip"],
                mt = "mp4",
                q = "1024x768",
                cap = false,
                lc = "en"
            };
            var encoding = new ASCIIEncoding();
            Byte[] dataBytes = encoding.GetBytes(JsonConvert.SerializeObject(playerParametersObj));

            Stream sendStream = http.GetRequestStream();
            sendStream.Write(dataBytes, 0, dataBytes.Length);
            sendStream.Close();
            var clipurl = "";

            // if the clip is not free, then the user must sign in first and set authentication cookie.
            if (!clip.UserMayViewClip)
            {
                if (HttpContext.Current.Application[Constants.AUTH_COOKIE] == null)
                    SetupAuthenticationCookie();
            }

            clipurl = RequestClipUrl(http);
            return clipurl;
        }

        private string RequestClipUrl(HttpWebRequest httpWebRequest)
        {
            if (HttpContext.Current.Application[Constants.AUTH_COOKIE] != null)
                httpWebRequest.Headers.Add("Cookie", HttpContext.Current.Application[Constants.AUTH_COOKIE].ToString());

            var response = httpWebRequest.GetResponse();
            var receiveStream = response.GetResponseStream();
            var sr = new StreamReader(receiveStream);
            var content = sr.ReadToEnd();

            return content;
        }

        private string LoginToPluralSight()
        {
            var req = (HttpWebRequest)WebRequest.Create(Constants.LOGIN_URL);
            req.AllowAutoRedirect = false;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            using (var writer = new StreamWriter(req.GetRequestStream()))
            {
                // write to the body of the POST request
                writer.Write("Username=" + Constants.USER_NAME + "&Password=" + Constants.PASSWORD);
            }

            req.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            req.Headers.Add("Origin", Constants.BASE_URL);
            req.Headers.Add("Cache-Control", "max-age=0");

            var response = req.GetResponse();
            var authCookie = response.Headers["Set-Cookie"];
            var receiveStream = response.GetResponseStream();
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string rawHtml = readStream.ReadToEnd();
            receiveStream.Close();
            readStream.Close();

            return authCookie;
        }

        private void SetupAuthenticationCookie()
        {
            var authCookie = LoginToPluralSight();
            if (string.IsNullOrWhiteSpace(authCookie) || authCookie.Contains("signin-errors"))
            {
                // ToDO: better handling of errors returned by pluralsight server.
                var resp = new HttpResponseMessage((HttpStatusCode)422)
                {
                    Content = new StringContent("Invalid credentials.")
                };
                throw new HttpResponseException(resp);
            }

            HttpContext.Current.Application[Constants.AUTH_COOKIE] = authCookie;
        }
    }
}