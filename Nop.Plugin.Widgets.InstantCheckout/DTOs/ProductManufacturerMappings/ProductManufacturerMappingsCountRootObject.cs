using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.ProductManufacturerMappings
{
    public class ProductManufacturerMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}