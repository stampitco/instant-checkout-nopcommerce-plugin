using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Orders;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Products;

namespace Nop.Plugin.Widgets.InstantCheckout.Helpers
{
    public interface IDTOHelper
    {
        ProductDto PrepareProductDTO(Product product);
        OrderDto PrepareOrderDTO(Order order);
    }
}