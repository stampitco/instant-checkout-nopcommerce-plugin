using Newtonsoft.Json;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Base;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs
{
    [JsonObject(Title = "attribute")]
    public class ProductItemAttributeDto : BaseDto
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
