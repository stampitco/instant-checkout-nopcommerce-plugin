using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.OrderItems
{
    public class OrderItemsCountRootObject
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }
}