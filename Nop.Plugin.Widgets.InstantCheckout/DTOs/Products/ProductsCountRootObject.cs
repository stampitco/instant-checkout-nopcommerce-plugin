using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.Products
{
    public class ProductsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}