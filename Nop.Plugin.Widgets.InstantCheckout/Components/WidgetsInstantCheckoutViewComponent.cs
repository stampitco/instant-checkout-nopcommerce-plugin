using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.InstantCheckout.Models;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.InstantCheckout.Components
{
    [ViewComponent(Name = "WidgetsInstantCheckout")]
    public class WidgetsInstantCheckoutViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;

        public WidgetsInstantCheckoutViewComponent(IStoreContext storeContext,
            IStaticCacheManager cacheManager,
            ISettingService settingService,
            IPictureService pictureService,
            IWebHelper webHelper)
        {
            _storeContext = storeContext;
            _cacheManager = cacheManager;
            _settingService = settingService;
            _pictureService = pictureService;
            _webHelper = webHelper;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var productDetailsModel = (ProductDetailsModel)additionalData;
            var instantCheckoutSettings = _settingService.LoadSetting<InstantCheckoutSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                MerchantId = instantCheckoutSettings.MerchantId,
                InstantCheckoutBaseUrl = instantCheckoutSettings.InstantCheckoutWebBaseUrl,
                ProductId = productDetailsModel.Id
            };

            if (string.IsNullOrEmpty(model.InstantCheckoutBaseUrl))
                return Content("");

            return View("~/Plugins/Widgets.InstantCheckout/Views/PublicInfo.cshtml", model);
        }
    }
}
