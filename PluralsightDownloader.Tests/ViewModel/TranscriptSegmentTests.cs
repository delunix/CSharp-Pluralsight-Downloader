using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PluralsightDownloader.Web.ViewModel.Tests
{
    [TestClass()]
    public class TranscriptSegmentTests
    {
        [TestMethod()]
        public void GetOffsetDisplayTimeStringTest()
        {
            var segment = new TranscriptSegment();
            var offset = segment.GetOffsetDisplayTimeString(1, 8);
            Assert.IsTrue(offset == "00:00:07,000");
        }

        [TestMethod()]
        public void DisplayTimeStringTest()
        {
            var segment = new TranscriptSegment();
            segment.DisplayTime = 15;
            var displayTimeString = segment.DisplayTimeString;
            Assert.IsTrue(displayTimeString == "00:00:15,000");
        }
    }
}