using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.InstantCheckout.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.InstantCheckout.MerchantId")]
        [UIHint("MerchantId")]
        public Guid MerchantId { get; set; }
        public bool MerchantId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.InstantCheckout.InstantCheckoutWebBaseUrl")]
        public string InstantCheckoutWebBaseUrl { get; set; }
        public bool InstantCheckoutWebBaseUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.InstantCheckout.InstantCheckoutApiBaseUrl")]
        public string InstantCheckoutApiBaseUrl { get; set; }
        public bool InstantCheckoutApiBaseUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.InstantCheckout.ConsumerKey")]
        [UIHint("ConsumerKey")]
        public Guid ConsumerKey { get; set; }
        public bool ConsumerKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.InstantCheckout.ConsumerSecret")]
        [UIHint("ConsumerSecret")]
        public Guid ConsumerSecret { get; set; }
        public bool ConsumerSecret_OverrideForStore { get; set; }
    }
}