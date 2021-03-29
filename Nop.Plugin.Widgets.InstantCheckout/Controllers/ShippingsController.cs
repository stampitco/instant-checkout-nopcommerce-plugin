using System.Net;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.InstantCheckout.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Nop.Core;
    using Nop.Core.Domain.Common;
    using Nop.Core.Domain.Orders;
    using Nop.Core.Domain.Shipping;
    using Nop.Plugin.Widgets.InstantCheckout.Attributes;
    using Nop.Plugin.Widgets.InstantCheckout.DTOs.Errors;
    using Nop.Plugin.Widgets.InstantCheckout.HeaderExtensions;
    using Nop.Plugin.Widgets.InstantCheckout.Helpers;
    using Nop.Plugin.Widgets.InstantCheckout.JSON.Serializers;
    using Nop.Plugin.Widgets.InstantCheckout.Models.ShippingsParameters;
    using Nop.Plugin.Widgets.InstantCheckout.Services;
    using Nop.Services.Catalog;
    using Nop.Services.Configuration;
    using Nop.Services.Directory;
    using Nop.Services.Orders;
    using Nop.Services.Shipping;
    using Nop.Services.Tax;
    using Nop.Web.Models.ShoppingCart;
    using static Nop.Plugin.Widgets.InstantCheckout.Models.ShippingsParameters.ShippingOptionsViewModel;

    public class ShippingsController : BaseApiController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IProductApiService _productApiService;
        private readonly ShippingSettings _shippingSettings;
        private readonly IShippingService _shippingService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IWorkContext _workContext;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;

        public ShippingsController(IJsonFieldsSerializer jsonFieldsSerializer,
                                  ISettingService settingService,
                                  IStoreContext storeContext,
                                  IProductApiService productApiService,
                                  ICustomerActivityService customerActivityService,
                                  ILocalizationService localizationService,
                                  IAclService aclService,
                                  IStoreMappingService storeMappingService,
                                  IStoreService storeService,
                                  ICustomerService customerService,
                                  IDiscountService discountService,
                                  IPictureService pictureService,
                                  IShippingService shippingService,
                                  ICountryService countryService,
                                  IStateProvinceService stateProvinceService,
                                  IWorkContext workContext,
                                  IOrderTotalCalculationService orderTotalCalculationService,
                                  ITaxService taxService,
                                  ICurrencyService currencyService,
                                  IPriceFormatter priceFormatter,
                                  ShippingSettings shippingSettings,
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _productApiService = productApiService;
            _shippingService = shippingService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _workContext = workContext;
            _orderTotalCalculationService = orderTotalCalculationService;
            _taxService = taxService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _shippingSettings = shippingSettings;
        }

        /// <summary>
        /// Retrieve product by specified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [Route("/api/ic/shippings")]
        [ProducesResponseType(typeof(EstimateShippingResultModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetShippings([FromBody] ShippingsParametersModel parameters)
        {
            var consumerKey = Request.Headers.GetInstantCheckoutConsumerKey();
            var consumerSecret = Request.Headers.GetInstantCheckoutConsumerSecret();

            if (consumerKey == Guid.Empty || consumerSecret == Guid.Empty)
            {
                return Unauthorized();
            }

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var instantCheckoutSettings = _settingService.LoadSetting<InstantCheckoutSettings>(storeScope);

            if (consumerKey != instantCheckoutSettings.ConsumerKey
                && consumerSecret != instantCheckoutSettings.ConsumerSecret)
            {
                return Unauthorized();
            }

            if (parameters.Ids == null && parameters.Ids.Any(x => x <= 0))
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var allProducts = _productApiService.GetProducts(parameters.Ids)
                                    .Where(p => _storeMappingService.Authorize(p));

            var cart = new List<ShoppingCartItem> { };

            foreach (var product in allProducts)
            {
                cart.Add(new ShoppingCartItem
                {
                    Customer = _workContext.CurrentCustomer,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1 //TODO ADD Proper Quantity by Product Check Shipping By Weight
                });
            }

            var model = new ShippingOptionsViewModel();


            var country = _countryService.GetCountryByTwoLetterIsoCode(parameters.CountryCode);
            var state = _stateProvinceService.GetStateProvinceByAbbreviation(parameters.StateCode);
            var address = new Address
            {
                CountryId = country.Id,
                Country = country,
                StateProvinceId = state != null ? state.Id : 0,
                StateProvince = state != null ? state : null,
                ZipPostalCode = parameters.ZipCode,
            };

            var getShippingOptionResponse = _shippingService.GetShippingOptions(cart, address, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
            if (getShippingOptionResponse.Success)
            {
                if (getShippingOptionResponse.ShippingOptions.Any())
                {
                    foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                    {
                        //calculate discounted and taxed rate
                        var shippingRate = _orderTotalCalculationService.AdjustShippingRate(shippingOption.Rate, cart, out var _);
                        shippingRate = _taxService.GetShippingPrice(shippingRate, _workContext.CurrentCustomer);
                        shippingRate = _currencyService.ConvertFromPrimaryStoreCurrency(shippingRate, _workContext.WorkingCurrency);

                        model.ShippingOptions.Add(new ShippingOptionModel
                        {
                            Name = shippingOption.Name,
                            Description = shippingOption.Description,
                            Price = shippingRate,
                            Plugin = shippingOption.ShippingRateComputationMethodSystemName
                        });
                    }
                }
            }
            else
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);

            if (_shippingSettings.AllowPickupInStore)
            {
                var pickupPointsResponse = _shippingService.GetPickupPoints(address, _workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                if (pickupPointsResponse.Success)
                {
                    if (pickupPointsResponse.PickupPoints.Any())
                    {
                        var soModel = new ShippingOptionModel
                        {
                            Name = _localizationService.GetResource("Checkout.PickupPoints"),
                            Description = _localizationService.GetResource("Checkout.PickupPoints.Description"),
                        };
                        var pickupFee = pickupPointsResponse.PickupPoints.Min(x => x.PickupFee);
                        if (pickupFee > 0)
                        {
                            pickupFee = _taxService.GetShippingPrice(pickupFee, _workContext.CurrentCustomer);
                            pickupFee = _currencyService.ConvertFromPrimaryStoreCurrency(pickupFee, _workContext.WorkingCurrency);
                        }
                        soModel.Price = pickupFee;
                        soModel.Plugin = "Pickup.PickupInStore";
                        model.ShippingOptions.Add(soModel);
                    }
                }
                else
                    foreach (var error in pickupPointsResponse.Errors)
                        model.Warnings.Add(error);
            }

            return Ok(model);
        }

    }
}