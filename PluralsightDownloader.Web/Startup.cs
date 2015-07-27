using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Owin;

namespace PluralsightDownloader.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var settings = new JsonSerializerSettings {ContractResolver = new SignalRContractResolver()};
            var serializer = JsonSerializer.Create(settings);
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => serializer);
            app.MapSignalR("/signalr", new HubConfiguration());
        }
    }
}