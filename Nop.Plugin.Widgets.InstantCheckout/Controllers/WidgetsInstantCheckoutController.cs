using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.InstantCheckout.Models;
using Nop.Plugin.Widgets.InstantCheckout.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.InstantCheckout.Controllers
{
    [Area(AreaNames.Admin)]
    public class WidgetsInstantCheckoutController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly InstantCheckoutManager _instantCheckoutManager;

        public WidgetsInstantCheckoutController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            InstantCheckoutManager instantCheckoutManager)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _instantCheckoutManager = instantCheckoutManager;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var instantCheckoutSettings = _settingService.LoadSetting<InstantCheckoutSettings>(storeScope);
            var model = new ConfigurationModel
            {
                MerchantId = instantCheckoutSettings.MerchantId,
                InstantCheckoutWebBaseUrl = instantCheckoutSettings.InstantCheckoutWebBaseUrl,
                InstantCheckoutApiBaseUrl = instantCheckoutSettings.InstantCheckoutApiBaseUrl,
                ConsumerKey = instantCheckoutSettings.ConsumerKey,
                ConsumerSecret = instantCheckoutSettings.ConsumerSecret,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.MerchantId_OverrideForStore = _settingService.SettingExists(instantCheckoutSettings, x => x.MerchantId, storeScope);
                model.InstantCheckoutWebBaseUrl_OverrideForStore = _settingService.SettingExists(instantCheckoutSettings, x => x.InstantCheckoutWebBaseUrl, storeScope);
                model.InstantCheckoutApiBaseUrl_OverrideForStore = _settingService.SettingExists(instantCheckoutSettings, x => x.InstantCheckoutApiBaseUrl, storeScope);
                model.ConsumerKey_OverrideForStore = _settingService.SettingExists(instantCheckoutSettings, x => x.ConsumerKey, storeScope);
                model.ConsumerSecret_OverrideForStore = _settingService.SettingExists(instantCheckoutSettings, x => x.ConsumerSecret, storeScope);
            }

            return View("~/Plugins/Widgets.InstantCheckout/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var instantCheckoutSettings = _settingService.LoadSetting<InstantCheckoutSettings>(storeScope);

            instantCheckoutSettings.MerchantId = model.MerchantId;
            instantCheckoutSettings.InstantCheckoutWebBaseUrl = model.InstantCheckoutWebBaseUrl;
            instantCheckoutSettings.InstantCheckoutApiBaseUrl = model.InstantCheckoutApiBaseUrl;
            instantCheckoutSettings.ConsumerKey = model.ConsumerKey;
            instantCheckoutSettings.ConsumerSecret = model.ConsumerSecret;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(instantCheckoutSettings, x => x.MerchantId, model.MerchantId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(instantCheckoutSettings, x => x.InstantCheckoutWebBaseUrl, model.InstantCheckoutWebBaseUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(instantCheckoutSettings, x => x.InstantCheckoutApiBaseUrl, model.InstantCheckoutApiBaseUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(instantCheckoutSettings, x => x.ConsumerKey, model.ConsumerKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(instantCheckoutSettings, x => x.ConsumerSecret, model.ConsumerSecret_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            // Install or Update plugin on Instant Checkout
            _instantCheckoutManager.InstallWidgetPlugin(instantCheckoutSettings.InstantCheckoutApiBaseUrl,
                instantCheckoutSettings.MerchantId.ToString(), instantCheckoutSettings.ConsumerKey.ToString(),
                instantCheckoutSettings.ConsumerSecret.ToString());

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}