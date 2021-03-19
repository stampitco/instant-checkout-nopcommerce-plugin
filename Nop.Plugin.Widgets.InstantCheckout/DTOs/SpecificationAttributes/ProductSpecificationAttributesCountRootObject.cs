using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.SpecificationAttributes
{
    public class ProductSpecificationAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}