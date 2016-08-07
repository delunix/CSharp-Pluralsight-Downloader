using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;

namespace PluralsightDownloader.Web.ViewModel
{
    public class TranscriptClip
    {
        public string Title { get; set; }
        public string PlayerUrl { get; set; }
        public TranscriptSegment[] Segments { get; set; }

        private string[] SplitLine(string line, int max)
        {
            var charCount = 0;
            return line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => (charCount += (((charCount%max) + w.Length + 1 >= max)
                    ? max - (charCount%max)
                    : 0) + w.Length + 1)/max)
                .Select(g => string.Join(" ", g.ToArray()))
                .ToArray();
        }

        private string AutoBreakLine(string line)
        {
            var newLine = new StringBuilder();
            var max = 43;
            if (line.Length > max)
            {
                foreach (string part in SplitLine(line, max))
                {
                    newLine.AppendLine(part);
                }
                return newLine.ToString();
            }
            return newLine.AppendLine(line).ToString();
        }

        private string GetLineString(string seq, string start, string end, string line)
        {
            return new StringBuilder()
                .AppendLine(seq)
                .AppendFormat("{0} --> {1}", start, end)
                .AppendLine()
                .AppendLine(AutoBreakLine(line))
                .ToString();
        }

        public string GetSrtString(long clipSeconds)
        {
            var srt = new StringBuilder().AppendLine();

            for (int i = 0; i < Segments.Length; i++)
            {
                var offSetTimeString = i + 1 < Segments.Length
                    ? Segments[i + 1].GetOffsetDisplayTimeString(1)
                    : Segments[i].GetOffsetDisplayTimeString(1, clipSeconds);

                var text = Segments[i].Text;
                srt.Append(GetLineString((i + 1).ToString(), Segments[i].DisplayTimeString, offSetTimeString, text));
            }

            return srt.ToString();
        }
    }
}