using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.InstantCheckout.Domain
{
    public class ApiSettings : ISettings
    {
        public bool EnableApi { get; set; }
        public bool AllowRequestsFromSwagger { get; set; }
        public bool EnableLogging { get; set; }
    }
}