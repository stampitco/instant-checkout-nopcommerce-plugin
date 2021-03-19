using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Orders;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Products;

namespace Nop.Plugin.Widgets.InstantCheckout.Helpers
{
    public interface IDTOHelper
    {
        ProductDto PrepareProductDTO(Product product);
        //CategoryDto PrepareCategoryDTO(Category category);
        OrderDto PrepareOrderDTO(Order order);
        //ShoppingCartItemDto PrepareShoppingCartItemDTO(ShoppingCartItem shoppingCartItem);
        //OrderItemDto PrepareOrderItemDTO(OrderItem orderItem);
        //StoreDto PrepareStoreDTO(Store store);
        //LanguageDto PrepateLanguageDto(Language language);
        //ProductAttributeDto PrepareProductAttributeDTO(ProductAttribute productAttribute);
        //ProductSpecificationAttributeDto PrepareProductSpecificationAttributeDto(ProductSpecificationAttribute productSpecificationAttribute);
        //SpecificationAttributeDto PrepareSpecificationAttributeDto(SpecificationAttribute specificationAttribute);
        //ManufacturerDto PrepareManufacturerDto(Manufacturer manufacturer);
    }
}