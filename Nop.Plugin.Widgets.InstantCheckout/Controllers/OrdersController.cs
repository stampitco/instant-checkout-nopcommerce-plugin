using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.InstantCheckout.Attributes;
using Nop.Plugin.Widgets.InstantCheckout.Constants;
using Nop.Plugin.Widgets.InstantCheckout.Delta;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.OrderItems;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Orders;
using Nop.Plugin.Widgets.InstantCheckout.Factories;
using Nop.Plugin.Widgets.InstantCheckout.Helpers;
using Nop.Plugin.Widgets.InstantCheckout.JSON.ActionResults;
using Nop.Plugin.Widgets.InstantCheckout.ModelBinders;
using Nop.Plugin.Widgets.InstantCheckout.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.InstantCheckout.Controllers
{
    using DTOs.Errors;
    using JSON.Serializers;
    using Nop.Core.Domain.Shipping;
    using Nop.Plugin.Widgets.InstantCheckout.HeaderExtensions;
    using Nop.Plugin.Widgets.InstantCheckout.Models.OrdersParameters;
    using Nop.Services.Configuration;
    using Nop.Services.Directory;

    public class OrdersController : BaseApiController
    {
        private readonly ISettingService _settingService;
        private readonly IOrderApiService _orderApiService;
        private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IShippingService _shippingService;
        private readonly IDTOHelper _dtoHelper;
        private readonly IProductAttributeConverter _productAttributeConverter;
        private readonly IStoreContext _storeContext;
        private readonly IFactory<Order> _factory;
        private readonly ICountryService _countryService;
        private readonly ILocalizationService _localizationService;

        public OrdersController(ISettingService settingService,
            IOrderApiService orderApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductService productService,
            IFactory<Order> factory,
            IOrderProcessingService orderProcessingService,
            IOrderService orderService,
            IShoppingCartService shoppingCartService,
            IGenericAttributeService genericAttributeService,
            IStoreContext storeContext,
            IShippingService shippingService,
            IPictureService pictureService,
            IDTOHelper dtoHelper,
            IProductAttributeConverter productAttributeConverter,
            ICountryService countryService)
            : base(jsonFieldsSerializer, aclService, customerService, storeMappingService,
                 storeService, discountService, customerActivityService, localizationService, pictureService)
        {
            _settingService = settingService;
            _orderApiService = orderApiService;
            _factory = factory;
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _shippingService = shippingService;
            _dtoHelper = dtoHelper;
            _productService = productService;
            _productAttributeConverter = productAttributeConverter;
            _countryService = countryService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Receive a list of all Orders
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Bad Request</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/ic/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrders(OrdersParametersModel parameters)
        {
            if (!IsRequestAuthorized())
            {
                return Unauthorized();
            }

            if (parameters.Page < Configurations.DEFAULT_PAGE_VALUE)
                return Error(HttpStatusCode.BadRequest, "page", "Invalid page parameter");

            if (parameters.Limit < Configurations.MIN_LIMIT || parameters.Limit > Configurations.MAX_LIMIT)
                return Error(HttpStatusCode.BadRequest, "page", "Invalid limit parameter");

            var storeId = _storeContext.CurrentStore.Id;

            var orders = _orderApiService.GetOrders(parameters.Ids, parameters.CreatedAtMin,
                parameters.CreatedAtMax,
                parameters.Limit, parameters.Page, parameters.SinceId,
                parameters.Status, parameters.PaymentStatus, parameters.ShippingStatus,
                parameters.CustomerId, storeId);

            IList<OrderDto> ordersAsDtos = orders.Select(x => _dtoHelper.PrepareOrderDTO(x)).ToList();

            var ordersRootObject = new OrdersRootObject()
            {
                Orders = ordersAsDtos
            };

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, parameters.Fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve order by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/ic/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderById(int id, string fields = "")
        {
            if (!IsRequestAuthorized())
            {
                return Unauthorized();
            }

            if (id <= 0)
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var order = _orderApiService.GetOrderById(id);

            if (order == null)
                return Error(HttpStatusCode.NotFound, "order", "not found");

            var ordersRootObject = new OrdersRootObject();

            var orderDto = _dtoHelper.PrepareOrderDTO(order);
            ordersRootObject.Orders.Add(orderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        /// <summary>
        /// Retrieve order by spcified id
        /// </summary>
        ///   /// <param name="id">Id of the order</param>
        /// <param name="fields">Fields from the order you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/ic/orders/byInstantCheckoutOrderId/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult GetOrderByInstantCheckoutOrderId(Guid id, string fields = "")
        {
            if (!IsRequestAuthorized())
            {
                return Unauthorized();
            }

            if (id == Guid.Empty)
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var order = _orderApiService.GetOrderByInstanCheckoutOrderId(id);

            if (order == null)
                return Error(HttpStatusCode.NotFound, "order", "not found");

            var ordersRootObject = new OrdersRootObject();

            var orderDto = _dtoHelper.PrepareOrderDTO(order);
            ordersRootObject.Orders.Add(orderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpPost]
        [Route("/api/ic/orders")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult CreateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            if (!IsRequestAuthorized())
            {
                return Unauthorized();
            }

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
                return Error();

            //if (orderDelta.Dto.CustomerId == null)
            //    return Error();

            //TODO Add Guest Customer
            var customer = _customerService.InsertGuestCustomer();

            // We doesn't have to check for value because this is done by the order validator.
            //var customer = CustomerService.GetCustomerById(orderDelta.Dto.CustomerId.Value);

            if (customer == null)
                return Error(HttpStatusCode.NotFound, "customer", "not found");

            // Save default checkout attribute

            //save checkout attributes
            var attributesXml = "<Attributes><CheckoutAttribute ID='1'><CheckoutAttributeValue><Value>1</Value></CheckoutAttributeValue></CheckoutAttribute></Attributes>";
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CheckoutAttributes, attributesXml, _storeContext.CurrentStore.Id);

            var shippingRequired = false;

            if (orderDelta.Dto.OrderItems != null)
            {
                var shouldReturnError = AddOrderItemsToCart(orderDelta.Dto.OrderItems, customer, orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id);
                if (shouldReturnError)
                    return Error(HttpStatusCode.BadRequest);

                shippingRequired = IsShippingAddressRequired(orderDelta.Dto.OrderItems);
            }

            if (shippingRequired)
            {
                var isValid = true;

                isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName,
                                            orderDelta.Dto.ShippingMethod,
                                            orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id,
                                            customer,
                                            BuildShoppingCartItemsFromOrderItemDtos(orderDelta.Dto.OrderItems.ToList(),
                                                                                    customer.Id,
                                                                                    orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id));

                if (!isValid)
                    return Error(HttpStatusCode.BadRequest);
            }

            var newOrder = _factory.Initialize();
            orderDelta.Merge(newOrder);

            var country = _countryService.GetCountryByTwoLetterIsoCode(orderDelta.Dto.ShippingAddress.CountryCode);
            newOrder.BillingAddress.CountryId = country.Id;
            newOrder.ShippingAddress.CountryId = country.Id;

            customer.BillingAddress = newOrder.BillingAddress;
            customer.ShippingAddress = newOrder.ShippingAddress;


            //Set the default Pickup Point

            if (orderDelta.Dto.ShippingRateComputationMethodSystemName == "Pickup.PickupInStore")
            {
                var pickupPoints = _shippingService.GetPickupPoints(customer.BillingAddress,
                customer, null, _storeContext.CurrentStore.Id).PickupPoints.ToList();
                var selectedPoint = pickupPoints.FirstOrDefault();
                if (selectedPoint == null)
                    return RedirectToRoute("CheckoutShippingAddress");

                var pickUpInStoreShippingOption = new ShippingOption
                {
                    Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                    Rate = selectedPoint.PickupFee,
                    Description = selectedPoint.Description,
                    ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName
                };

                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, _storeContext.CurrentStore.Id);
                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SelectedPickupPointAttribute, selectedPoint, _storeContext.CurrentStore.Id);
            }

            // If the customer has something in the cart it will be added too. Should we clear the cart first? 
            newOrder.Customer = customer;

            // The default value will be the currentStore.id, but if it isn't passed in the json we need to set it by hand.
            if (!orderDelta.Dto.StoreId.HasValue)
                newOrder.StoreId = _storeContext.CurrentStore.Id;

            var placeOrderResult = PlaceOrder(orderDelta.Dto.OrderGuid, newOrder, customer);

            if (!placeOrderResult.Success)
            {
                foreach (var error in placeOrderResult.Errors)
                    ModelState.AddModelError("order placement", error);

                return Error(HttpStatusCode.BadRequest);
            }

            _customerActivityService.InsertActivity("AddNewOrder",
                 base._localizationService.GetResource("ActivityLog.AddNewOrder"), newOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = _dtoHelper.PrepareOrderDTO(placeOrderResult.PlacedOrder);
            placedOrderDto.OrderGuid = placeOrderResult.PlacedOrder.OrderGuid;

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/ic/orders/{id}/cancel")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [GetRequestsErrorInterceptorActionFilter]
        public IActionResult CancelOrder(int id)
        {
            if (!IsRequestAuthorized())
            {
                return Unauthorized();
            }

            if (id <= 0)
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");

            var orderToCancel = _orderApiService.GetOrderById(id);

            if (orderToCancel == null)
                return Error(HttpStatusCode.NotFound, "order", "not found");

            _orderProcessingService.CancelOrder(orderToCancel, false);

            var ordersRootObject = new OrdersRootObject();
            var cancelledOrderDto = _dtoHelper.PrepareOrderDTO(orderToCancel);
            ordersRootObject.Orders.Add(cancelledOrderDto);
            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/ic/orders/{id}")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateOrder([ModelBinder(typeof(JsonModelBinder<OrderDto>))] Delta<OrderDto> orderDelta)
        {
            if (!IsRequestAuthorized())
            {
                return Unauthorized();
            }

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
                return Error();

            var currentOrder = _orderApiService.GetOrderById(orderDelta.Dto.Id);

            if (currentOrder == null)
                return Error(HttpStatusCode.NotFound, "order", "not found");

            var customer = currentOrder.Customer;

            var shippingRequired = currentOrder.OrderItems.Any(item => !item.Product.IsFreeShipping);

            if (shippingRequired)
            {
                var isValid = true;

                if (!string.IsNullOrEmpty(orderDelta.Dto.ShippingRateComputationMethodSystemName) ||
                    !string.IsNullOrEmpty(orderDelta.Dto.ShippingMethod))
                {
                    var storeId = orderDelta.Dto.StoreId ?? _storeContext.CurrentStore.Id;

                    isValid &= SetShippingOption(orderDelta.Dto.ShippingRateComputationMethodSystemName ?? currentOrder.ShippingRateComputationMethodSystemName,
                        orderDelta.Dto.ShippingMethod,
                        storeId,
                        customer, BuildShoppingCartItemsFromOrderItems(currentOrder.OrderItems.ToList(), customer.Id, storeId));
                }

                if (isValid)
                {
                    //Set the default Pickup Point

                    if (orderDelta.Dto.ShippingRateComputationMethodSystemName == "Pickup.PickupInStore")
                    {
                        var pickupPoints = _shippingService.GetPickupPoints(customer.BillingAddress,
                        customer, null, _storeContext.CurrentStore.Id).PickupPoints.ToList();
                        var selectedPoint = pickupPoints.FirstOrDefault();
                        if (selectedPoint == null)
                            return RedirectToRoute("CheckoutShippingAddress");

                        var pickUpInStoreShippingOption = new ShippingOption
                        {
                            Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                            Rate = selectedPoint.PickupFee,
                            Description = selectedPoint.Description,
                            ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName
                        };

                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, _storeContext.CurrentStore.Id);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SelectedPickupPointAttribute, selectedPoint, _storeContext.CurrentStore.Id);
                    }
                    currentOrder.ShippingMethod = orderDelta.Dto.ShippingMethod;
                }
                else
                    return Error(HttpStatusCode.BadRequest);
            }

            orderDelta.Merge(currentOrder);

            customer.BillingAddress = currentOrder.BillingAddress;
            customer.ShippingAddress = currentOrder.ShippingAddress;

            _orderService.UpdateOrder(currentOrder);

            _customerActivityService.InsertActivity("UpdateOrder",
                 base._localizationService.GetResource("ActivityLog.UpdateOrder"), currentOrder);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = _dtoHelper.PrepareOrderDTO(currentOrder);
            placedOrderDto.ShippingMethod = orderDelta.Dto.ShippingMethod;

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/ic/orders/{id}/paid")]
        [ProducesResponseType(typeof(OrdersRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public IActionResult UpdateOrderAsPaid(int id)
        {
            if (!IsRequestAuthorized())
            {
                return Unauthorized();
            }

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
                return Error();

            var orderPaid = MarkOrderAsPaid(id);

            if (!orderPaid)
            {
                return Error(HttpStatusCode.NotFound, "order", "not found");
            }

            var currentOrder = _orderApiService.GetOrderById(id);

            var ordersRootObject = new OrdersRootObject();

            var placedOrderDto = _dtoHelper.PrepareOrderDTO(currentOrder);

            ordersRootObject.Orders.Add(placedOrderDto);

            var json = _jsonFieldsSerializer.Serialize(ordersRootObject, string.Empty);

            return new RawJsonActionResult(json);
        }

        private bool SetShippingOption(string shippingRateComputationMethodSystemName, string shippingOptionName, int storeId, Customer customer, List<ShoppingCartItem> shoppingCartItems)
        {
            var isValid = true;

            if (string.IsNullOrEmpty(shippingRateComputationMethodSystemName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_rate_computation_method_system_name",
                    "Please provide shipping_rate_computation_method_system_name");
            }
            else if (string.IsNullOrEmpty(shippingOptionName))
            {
                isValid = false;

                ModelState.AddModelError("shipping_option_name", "Please provide shipping_option_name");
            }
            else
            {
                var shippingOptionResponse = _shippingService.GetShippingOptions(shoppingCartItems, customer.ShippingAddress, customer,
                        shippingRateComputationMethodSystemName, storeId);

                if (shippingOptionResponse.Success)
                {
                    var shippingOptions = shippingOptionResponse.ShippingOptions.ToList();

                    var shippingOption = shippingOptions
                        .Find(so => !string.IsNullOrEmpty(so.Name) && so.Name.Equals(shippingOptionName, StringComparison.InvariantCultureIgnoreCase));

                    _genericAttributeService.SaveAttribute(customer,
                        NopCustomerDefaults.SelectedShippingOptionAttribute,
                        shippingOption, storeId);
                }
                else
                {
                    isValid = false;

                    foreach (var errorMessage in shippingOptionResponse.Errors)
                        ModelState.AddModelError("shipping_option", errorMessage);
                }
            }

            return isValid;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItems(List<OrderItem> orderItems, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItems)
                shoppingCartItems.Add(new ShoppingCartItem()
                {
                    ProductId = orderItem.ProductId,
                    CustomerId = customerId,
                    Quantity = orderItem.Quantity,
                    RentalStartDateUtc = orderItem.RentalStartDateUtc,
                    RentalEndDateUtc = orderItem.RentalEndDateUtc,
                    StoreId = storeId,
                    Product = orderItem.Product,
                    ShoppingCartType = ShoppingCartType.ShoppingCart
                });

            return shoppingCartItems;
        }

        private List<ShoppingCartItem> BuildShoppingCartItemsFromOrderItemDtos(List<OrderItemDto> orderItemDtos, int customerId, int storeId)
        {
            var shoppingCartItems = new List<ShoppingCartItem>();

            foreach (var orderItem in orderItemDtos)
                if (orderItem.ProductId != null)
                    shoppingCartItems.Add(new ShoppingCartItem()
                    {
                        ProductId = orderItem.ProductId.Value, // required field
                        CustomerId = customerId,
                        Quantity = orderItem.Quantity ?? 1,
                        RentalStartDateUtc = orderItem.RentalStartDateUtc,
                        RentalEndDateUtc = orderItem.RentalEndDateUtc,
                        StoreId = storeId,
                        Product = _productService.GetProductById(orderItem.ProductId.Value),
                        ShoppingCartType = ShoppingCartType.ShoppingCart
                    });

            return shoppingCartItems;
        }

        private PlaceOrderResult PlaceOrder(Guid instantCheckoutOrderId, Order newOrder, Customer customer)
        {
            var processPaymentRequest = new ProcessPaymentRequest
            {
                OrderGuid = instantCheckoutOrderId,
                StoreId = newOrder.StoreId,
                CustomerId = customer.Id,
                PaymentMethodSystemName = newOrder.PaymentMethodSystemName
            };


            var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);

            return placeOrderResult;
        }

        public bool MarkOrderAsPaid(int id)
        {
            //try to get an order with the specified id
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return false;

            //a vendor does not have access to this functionality
            //if (_workContext.CurrentVendor != null)
            //    return RedirectToAction("Edit", "Order", new { id });

            try
            {
                _orderProcessingService.MarkOrderAsPaid(order);
                //LogEditOrder(order.Id);

                //prepare model
                //var model = _orderModelFactory.PrepareOrderModel(null, order);

                return true;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        private bool IsShippingAddressRequired(ICollection<OrderItemDto> orderItems)
        {
            var shippingAddressRequired = false;

            foreach (var orderItem in orderItems)
                if (orderItem.ProductId != null)
                {
                    var product = _productService.GetProductById(orderItem.ProductId.Value);

                    shippingAddressRequired |= product.IsShipEnabled;
                }

            return shippingAddressRequired;
        }

        private bool AddOrderItemsToCart(ICollection<OrderItemDto> orderItems, Customer customer, int storeId)
        {
            var shouldReturnError = false;

            foreach (var orderItem in orderItems)
                if (orderItem.ProductId != null)
                {
                    var product = _productService.GetProductById(orderItem.ProductId.Value);

                    if (!product.IsRental)
                    {
                        orderItem.RentalStartDateUtc = null;
                        orderItem.RentalEndDateUtc = null;
                    }

                    var attributesXml = _productAttributeConverter.ConvertToXml(orderItem.Attributes.ToList(), product.Id);

                    var errors = _shoppingCartService.AddToCart(customer, product,
                        ShoppingCartType.ShoppingCart, storeId, attributesXml,
                        0M, orderItem.RentalStartDateUtc, orderItem.RentalEndDateUtc,
                        orderItem.Quantity ?? 1);

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                            ModelState.AddModelError("order", error);

                        shouldReturnError = true;
                    }
                }

            return shouldReturnError;
        }

        private bool IsRequestAuthorized()
        {
            var consumerKey = Request.Headers.GetInstantCheckoutConsumerKey();
            var consumerSecret = Request.Headers.GetInstantCheckoutConsumerSecret();

            if (consumerKey == Guid.Empty || consumerSecret == Guid.Empty)
            {
                return false;
            }

            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var instantCheckoutSettings = _settingService.LoadSetting<InstantCheckoutSettings>(storeScope);

            if (consumerKey != instantCheckoutSettings.ConsumerKey
                && consumerSecret != instantCheckoutSettings.ConsumerSecret)
            {
                return false;
            }

            return true;
        }
    }
}