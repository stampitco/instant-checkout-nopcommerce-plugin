using System.Collections.Generic;
using Nop.Plugin.Widgets.InstantCheckout.DTOs;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    public interface IProductAttributeConverter
    {
        List<ProductItemAttributeDto> Parse(string attributesXml);
        string ConvertToXml(List<ProductItemAttributeDto> attributeDtos, int productId);
    }
}
