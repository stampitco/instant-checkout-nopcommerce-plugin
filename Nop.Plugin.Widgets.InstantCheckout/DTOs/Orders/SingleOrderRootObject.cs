using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.Orders
{
    public class SingleOrderRootObject
    {
        [JsonProperty("order")]
        public OrderDto Order { get; set; }
    }
}