using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace PluralsightDownloader.Web.Extensions
{
    public static class StringExtentions
    {
        public static string ToValidFileName(this string fileName)
        {
            byte[] bytes = Encoding.Default.GetBytes(fileName);
            fileName = Encoding.UTF8.GetString(bytes);

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                fileName = fileName.Replace(c.ToString(), "");
            
          return fileName;
        }
    }
}