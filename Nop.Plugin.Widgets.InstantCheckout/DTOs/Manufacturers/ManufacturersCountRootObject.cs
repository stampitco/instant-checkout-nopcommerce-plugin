using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.Manufacturers
{
    public class ManufacturersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}