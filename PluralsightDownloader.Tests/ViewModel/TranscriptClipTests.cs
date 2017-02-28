using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluralsightDownloader.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using Newtonsoft.Json;

namespace PluralsightDownloader.Tests.ViewModel
{
    [TestClass()]
    public class TranscriptClipTests
    {
        private Transcript transcript;
        private Course course;

        [TestInitialize]
        public void Setup()
        {
            var examplesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\ExampleJson");
            FileIOPermission fileIoPermission = new FileIOPermission(FileIOPermissionAccess.Read, examplesFolder);
            fileIoPermission.Demand();

            var transcriptJson = File.ReadAllText(examplesFolder + "\\Transcript.json");
            transcript = JsonConvert.DeserializeObject<Transcript>(transcriptJson);

            var courseJson = File.ReadAllText(examplesFolder + "\\Course.json");
            course = JsonConvert.DeserializeObject<Course>(courseJson);

            var courseModulesJson = File.ReadAllText(examplesFolder + "\\CourseModules.json");
            course.Content = JsonConvert.DeserializeObject<CourseContent>(courseModulesJson);
        }

        [TestMethod()]
        public void GetSrtStringTest()
        {
            
            var clip = transcript.Modules.First().Clips.First();
            var courseClip = course.Content.Modules.First().Clips.First();
            var srtString = clip.GetSrtString(courseClip.DurationSeconds);
//            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\ViewModel\\srtTest.srt", srtString);
            var srtTest = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "\\ViewModel\\srtTest.srt");
            Assert.IsTrue(srtString == srtTest);
        }
    }
}