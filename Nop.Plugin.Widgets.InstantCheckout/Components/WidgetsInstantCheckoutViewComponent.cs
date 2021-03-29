using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.InstantCheckout.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.InstantCheckout.Components
{
    [ViewComponent(Name = "WidgetsInstantCheckout")]
    public class WidgetsInstantCheckoutViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;

        public WidgetsInstantCheckoutViewComponent(IStoreContext storeContext,
            IWorkContext workContext,
            ISettingService settingService)
        {
            _storeContext = storeContext;
            _workContext = workContext;
            _settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            PublicInfoModel model = null;
            var instantCheckoutSettings = _settingService.LoadSetting<InstantCheckoutSettings>(_storeContext.CurrentStore.Id);

            if (additionalData != null)
            {
                var productDetailsModel = (ProductDetailsModel)additionalData;


                model = new PublicInfoModel
                {
                    MerchantId = instantCheckoutSettings.MerchantId,
                    InstantCheckoutBaseUrl = instantCheckoutSettings.InstantCheckoutWebBaseUrl,
                    ProductId = productDetailsModel.Id,
                    IsCart = false
                };
            }
            else
            {
                model = new PublicInfoModel
                {
                    MerchantId = instantCheckoutSettings.MerchantId,
                    InstantCheckoutBaseUrl = instantCheckoutSettings.InstantCheckoutWebBaseUrl,
                    IsCart = true,
                    Products = GetProductsFromCart()
                };
            }

            if (string.IsNullOrEmpty(model?.InstantCheckoutBaseUrl))
                return Content("");

            return View("~/Plugins/Widgets.InstantCheckout/Views/PublicInfo.cshtml", model);
        }

        private string GetProductsFromCart()
        {
            var customerShoppingCart = GetCart();

            var productQuantity = new ProductQuantity[customerShoppingCart.Count()];

            for (int i = 0; i < customerShoppingCart.Count(); i++)
            {
                productQuantity[i] = new ProductQuantity
                {
                    i = customerShoppingCart[i].ProductId,
                    q = customerShoppingCart[i].Quantity
                };
            }

            return JsonConvert.SerializeObject(productQuantity);
        }

        private IList<ShoppingCartItem> GetCart()
        {
            return _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .Where(sci => sci.StoreId == _storeContext.CurrentStore.Id)
                .ToList();
        }
    }
}
