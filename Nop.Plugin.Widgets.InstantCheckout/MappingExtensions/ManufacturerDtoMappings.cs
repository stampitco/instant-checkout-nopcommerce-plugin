using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.InstantCheckout.AutoMapper;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Manufacturers;

namespace Nop.Plugin.Widgets.InstantCheckout.MappingExtensions
{
    public static class ManufacturerDtoMappings
    {
        public static ManufacturerDto ToDto(this Manufacturer manufacturer)
        {
            return manufacturer.MapTo<Manufacturer, ManufacturerDto>();
        }
    }
}