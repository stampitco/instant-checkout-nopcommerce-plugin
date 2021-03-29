using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Orders;

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


        public async Task InstantCheckoutWebHooks(string baseUrl, string merchantId, string webhookTopic, OrderDto bodyDto)
        {
            //configure client
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(baseUrl);
            }

            if (!_httpClient.DefaultRequestHeaders.Contains("X-IC-AppKey"))
            {
                _httpClient.DefaultRequestHeaders.Add("X-IC-AppKey", merchantId);
            }

            if (!_httpClient.DefaultRequestHeaders.Contains("x-np-webhook-topic"))
            {
                _httpClient.DefaultRequestHeaders.Add("x-np-webhook-topic", webhookTopic);
            }

            try
            {
                var requestContent = new StringContent(JsonConvert.SerializeObject(bodyDto), Encoding.UTF8, MimeTypes.ApplicationJson);
                var response = await _httpClient.PostAsync("api/webhooks/nopcommerce", requestContent);

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
