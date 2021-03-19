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
    using Nop.Plugin.Widgets.InstantCheckout.Attributes;
    using Nop.Plugin.Widgets.InstantCheckout.Constants;
    using Nop.Plugin.Widgets.InstantCheckout.DTOs.Errors;
    using Nop.Plugin.Widgets.InstantCheckout.DTOs.Products;
    using Nop.Plugin.Widgets.InstantCheckout.HeaderExtensions;
    using Nop.Plugin.Widgets.InstantCheckout.Helpers;
    using Nop.Plugin.Widgets.InstantCheckout.JSON.ActionResults;
    using Nop.Plugin.Widgets.InstantCheckout.JSON.Serializers;
    using Nop.Plugin.Widgets.InstantCheckout.Models.ProductsParameters;
    using Nop.Plugin.Widgets.InstantCheckout.Services;
    using Nop.Services.Configuration;

    public class ProductsController : BaseApiController
    {
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IProductApiService _productApiService;
        private readonly IDTOHelper _dtoHelper;

        public ProductsController(IJsonFieldsSerializer jsonFieldsSerializer,
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
                                  IDTOHelper dtoHelper) : base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _settingService = settingService;
            _storeContext = storeContext;
            _productApiService = productApiService;
            _dtoHelper = dtoHelper;
        }


        /// <summary>
        /// Receive a list of all products
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/ic/products")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProducts(ProductsParametersModel parameters)
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

            if (parameters.Limit < Configurations.MinLimit || parameters.Limit > Configurations.MaxLimit)
            {
                return Error(HttpStatusCode.BadRequest, "limit", "invalid limit parameter");
            }

            if (parameters.Page < Configurations.DefaultPageValue)
            {
                return Error(HttpStatusCode.BadRequest, "page", "invalid page parameter");
            }

            var allProducts = _productApiService.GetProducts(parameters.Ids, parameters.CreatedAtMin, parameters.CreatedAtMax, parameters.UpdatedAtMin,
                                                                        parameters.UpdatedAtMax, parameters.Limit, parameters.Page, parameters.SinceId,
                                                                        parameters.PublishedStatus)
                                                .Where(p => StoreMappingService.Authorize(p));

            IList<ProductDto> productsAsDtos = allProducts.Select(product => _dtoHelper.PrepareProductDTO(product)).ToList();

            var productsRootObject = new ProductsRootObjectDto()
            {
                Products = productsAsDtos
            };

            var json = JsonFieldsSerializer.Serialize(productsRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve product by specified id
        /// </summary>
        /// <param name="id">Id of the product</param>
        /// <param name="fields">Fields from the product you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/ic/products/{id}")]
        [ProducesResponseType(typeof(ProductsRootObjectDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetProductById(int id, string fields = "")
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

            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var product = _productApiService.GetProductById(id);

            if (product == null)
            {
                return Error(HttpStatusCode.NotFound, "product", "not found");
            }

            var productDto = _dtoHelper.PrepareProductDTO(product);

            var productsRootObject = new ProductsRootObjectDto();

            productsRootObject.Products.Add(productDto);

            var json = JsonFieldsSerializer.Serialize(productsRootObject, fields);

            return new RawJsonActionResult(json);
        }

    }
}