using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.Customers
{
    public class CustomersCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}