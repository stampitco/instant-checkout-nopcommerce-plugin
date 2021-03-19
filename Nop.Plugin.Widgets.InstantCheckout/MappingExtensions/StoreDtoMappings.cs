using Nop.Core.Domain.Stores;
using Nop.Plugin.Widgets.InstantCheckout.AutoMapper;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Stores;

namespace Nop.Plugin.Widgets.InstantCheckout.MappingExtensions
{
    public static class StoreDtoMappings
    {
        public static StoreDto ToDto(this Store store)
        {
            return store.MapTo<Store, StoreDto>();
        }
    }
}
