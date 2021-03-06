using System;
using System.Collections.Generic;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Widgets.InstantCheckout.Constants;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    public interface IOrderApiService
    {
        IList<Order> GetOrdersByCustomerId(int customerId);

        IList<Order> GetOrders(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
                               int limit = Configurations.DEFAULT_LIMIT, int page = Configurations.DEFAULT_PAGE_VALUE,
                               int sinceId = Configurations.DEFAULT_SINCE_ID, OrderStatus? status = null, PaymentStatus? paymentStatus = null,
                               ShippingStatus? shippingStatus = null, int? customerId = null, int? storeId = null);

        Order GetOrderById(int orderId);
        Order GetOrderByInstanCheckoutOrderId(Guid instantCheckoutOrderId);

        int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
                           PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
                           int? customerId = null, int? storeId = null);
    }
}