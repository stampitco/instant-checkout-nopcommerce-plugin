using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.InstantCheckout
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class InstantCheckoutPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;


        public InstantCheckoutPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.ProductDetailsInsideOverviewButtonsBefore };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsInstantCheckout/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsInstantCheckout";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new InstantCheckoutSettings
            {
                MerchantId = Guid.NewGuid(),
                ConsumerKey = Guid.NewGuid(),
                ConsumerSecret = Guid.NewGuid(),
                InstantCheckoutWebBaseUrl = "https://c.instantcheckout.io",
                InstantCheckoutApiBaseUrl = "https://api.instantcheckout.io",
            };
            _settingService.SaveSetting(settings);

            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.MerchantId", "Merchant Identifier");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.MerchantId.Hint", "Merchant Identifier provided by Instant Checkout Team.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutWebBaseUrl", "Instant Checkout Web Base URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutWebBaseUrl.Hint", "The Instant Checkout Web base URL for the checkouts provided by Instant Checkout Team.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutApiBaseUrl", "Instant Checkout API Base URL");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutApiBaseUrl.Hint", "The Instant Checkout API base URL for the checkouts provided by Instant Checkout Team.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerKey", "Consumer Key for the API");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerKey.Hint", "Consumer Key for the API access from external calls.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerSecret", "Consumer Secret for the API");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerSecret.Hint", "Consumer Secret for the API access from external calls.");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<InstantCheckoutSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.MerchantId");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.MerchantId.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutWebBaseUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutWebBaseUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutApiBaseUrl");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.InstantCheckoutApiBaseUrl.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerKey");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerKey.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.Widgets.InstantCheckout.ConsumerSecret.Hint");

            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}