using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.Base
{
    public abstract class BaseDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }
    }
}