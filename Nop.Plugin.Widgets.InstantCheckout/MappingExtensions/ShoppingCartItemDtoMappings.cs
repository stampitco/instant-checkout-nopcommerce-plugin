using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.InstantCheckout.AutoMapper;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.ShoppingCarts;

namespace Nop.Plugin.Widgets.InstantCheckout.MappingExtensions
{
    public static class ShoppingCartItemDtoMappings
    {
        public static ShoppingCartItemDto ToDto(this ShoppingCartItem shoppingCartItem)
        {
            return shoppingCartItem.MapTo<ShoppingCartItem, ShoppingCartItemDto>();
        }
    }
}