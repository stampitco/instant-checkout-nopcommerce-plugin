using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Widgets.InstantCheckout.Constants;
using Nop.Plugin.Widgets.InstantCheckout.DataStructures;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    public class OrderApiService : IOrderApiService
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderApiService(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IList<Order> GetOrdersByCustomerId(int customerId)
        {
            var query = from order in _orderRepository.Table
                        where order.CustomerId == customerId && !order.Deleted
                        orderby order.Id
                        select order;

            return new ApiList<Order>(query, 0, Configurations.MAX_LIMIT);
        }

        public IList<Order> GetOrders(IList<int> ids = null, DateTime? createdAtMin = null, DateTime? createdAtMax = null,
           int limit = Configurations.DEFAULT_LIMIT, int page = Configurations.DEFAULT_PAGE_VALUE, int sinceId = Configurations.DEFAULT_SINCE_ID,
           OrderStatus? status = null, PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, int? customerId = null,
           int? storeId = null)
        {
            var query = GetOrdersQuery(createdAtMin, createdAtMax, status, paymentStatus, shippingStatus, ids, customerId, storeId);

            if (sinceId > 0)
            {
                query = query.Where(order => order.Id > sinceId);
            }

            return new ApiList<Order>(query, page - 1, limit);
        }

        public Order GetOrderById(int orderId)
        {
            if (orderId <= 0)
                return null;

            return _orderRepository.Table.FirstOrDefault(order => order.Id == orderId && !order.Deleted);
        }

        public Order GetOrderByInstanCheckoutOrderId(Guid instantCheckoutOrderId)
        {
            if (instantCheckoutOrderId == Guid.Empty)
                return null;

            return _orderRepository.Table.FirstOrDefault(order => order.OrderGuid == instantCheckoutOrderId && !order.Deleted);
        }

        public int GetOrdersCount(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
                                 PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null,
                                 int? customerId = null, int? storeId = null)
        {
            var query = GetOrdersQuery(createdAtMin, createdAtMax, status, paymentStatus, shippingStatus, customerId: customerId, storeId: storeId);

            return query.Count();
        }

        private IQueryable<Order> GetOrdersQuery(DateTime? createdAtMin = null, DateTime? createdAtMax = null, OrderStatus? status = null,
            PaymentStatus? paymentStatus = null, ShippingStatus? shippingStatus = null, IList<int> ids = null,
            int? customerId = null, int? storeId = null)
        {
            var query = _orderRepository.Table;

            if (customerId != null)
            {
                query = query.Where(order => order.CustomerId == customerId);
            }

            if (ids != null && ids.Count > 0)
            {
                query = query.Where(c => ids.Contains(c.Id));
            }

            if (status != null)
            {
                query = query.Where(order => order.OrderStatusId == (int)status);
            }

            if (paymentStatus != null)
            {
                query = query.Where(order => order.PaymentStatusId == (int)paymentStatus);
            }

            if (shippingStatus != null)
            {
                query = query.Where(order => order.ShippingStatusId == (int)shippingStatus);
            }

            query = query.Where(order => !order.Deleted);

            if (createdAtMin != null)
            {
                query = query.Where(order => order.CreatedOnUtc > createdAtMin.Value.ToUniversalTime());
            }

            if (createdAtMax != null)
            {
                query = query.Where(order => order.CreatedOnUtc < createdAtMax.Value.ToUniversalTime());
            }

            if (storeId != null)
            {
                query = query.Where(order => order.StoreId == storeId);
            }

            query = query.OrderBy(order => order.Id);

            query = query.Include(c => c.Customer);
            query = query.Include(c => c.BillingAddress);
            query = query.Include(c => c.ShippingAddress);
            query = query.Include(c => c.PickupAddress);
            query = query.Include(c => c.OrderNotes);
            query = query.Include(c => c.OrderItems);
            query = query.Include(c => c.Shipments);

            return query;
        }
    }
}