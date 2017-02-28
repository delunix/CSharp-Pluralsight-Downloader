using System.Collections.Generic;

namespace PluralsightDownloader.Web.ViewModel
{
    public class ClipDownloadData
    {
        public List<ClipDownloadUrl> URLs { get; set; }

        public List<ClipDownloadCaption> Captions { get; set; }
    }
}