using Newtonsoft.Json;
using Nop.Plugin.Widgets.InstantCheckout.Attributes;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs.Images
{
    [ImageValidation]
    public class ImageMappingDto : ImageDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("picture_id")]
        public int PictureId { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }
    }
}