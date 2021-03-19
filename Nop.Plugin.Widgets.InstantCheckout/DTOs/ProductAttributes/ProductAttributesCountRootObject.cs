using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.ProductAttributes
{
    public class ProductAttributesCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}