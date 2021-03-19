using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Nop.Core;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    public class InstantCheckoutService
    {
        #region Fields

        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public InstantCheckoutService(HttpClient client)
        {
            client.DefaultRequestHeaders.Add(HeaderNames.Accept, MimeTypes.ApplicationJson);
            _httpClient = client;
        }

        #endregion

        #region Properties

        public string BaseAddress => _httpClient.BaseAddress.ToString();

        #endregion

        #region Methods

        public async Task InstallPluginOnInstantCheckout(string baseUrl, string merchantId, string consumerKey, string consumerSecret,
            string storeName, string website, string platformVersion)
        {
            //configure client
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-IC-AppKey", merchantId);

            try
            {
                //get response
                var request = new
                {
                    ConsumerKey = consumerKey,
                    ConsumerSecret = consumerSecret,
                    Platform = "NopCommerce",
                    PlatformVersion = platformVersion,
                    WebSiteUrl = website,
                    PluginVersion = "1.00",
                    StoreName = storeName
                };
                var requestContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.ApplicationJson);
                var response = await _httpClient.PostAsync("api/instant-checkout/installations", requestContent);

                //return confirmation
                var responseContent = await response.Content.ReadAsStringAsync();
                var accessTokenResponse = JsonConvert.DeserializeObject(responseContent);
            }
            catch (AggregateException exception)
            {
                //rethrow actual exception
                throw exception.InnerException;
            }
        }

        #endregion
    }
}
