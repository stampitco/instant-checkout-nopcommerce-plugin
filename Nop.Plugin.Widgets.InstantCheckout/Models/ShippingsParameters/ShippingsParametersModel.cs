using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nop.Plugin.Widgets.InstantCheckout.Models.ShippingsParameters
{
    public class ShippingsParametersModel
    {
        /// <summary>
        /// A comma-separated list of order ids
        /// </summary>
        [JsonProperty("ids")]
        public List<int> Ids { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("state_code")]
        public string StateCode { get; set; }

        [JsonProperty("zip_code")]
        public string ZipCode { get; set; }
    }
}