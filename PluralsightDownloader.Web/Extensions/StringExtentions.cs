using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PluralsightDownloader.Web.Extensions
{
    public static class StringExtentions
    {
        public static string ToValidFileName(this string fileName)
        {
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c.ToString(), "");

            return fileName;
        }
    }
}