using System;
using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.InstantCheckout
{
    public class InstantCheckoutSettings : ISettings
    {
        public Guid MerchantId { get; set; }
        public string InstantCheckoutWebBaseUrl { get; set; }
        public string InstantCheckoutApiBaseUrl { get; set; }
        public Guid ConsumerKey { get; set; }
        public Guid ConsumerSecret { get; set; }
    }
}