using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.ProductCategoryMappings
{
    public class ProductCategoryMappingsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}