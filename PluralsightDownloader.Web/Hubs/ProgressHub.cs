using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using PluralsightDownloader.Web.ViewModel;

namespace PluralsightDownloader.Web.Hubs
{
    [HubName("ProgressHub")]
    public class ProgressHub : Hub
    {
        public void ReportProgress(ProgressArgs progress)
        {
            Clients.All.updateProgress(progress);
        }
    }
}