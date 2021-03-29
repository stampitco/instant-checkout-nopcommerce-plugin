using System;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Widgets.InstantCheckout.HeaderExtensions
{
    public static class HttpHeaderDictionaryExtensions
    {
        internal const string INSTANT_CHECKOUT_CONSUMER_KEY_HEADER = "X-IC-ConsumerKey";
        internal const string INSTANT_CHECKOUT_CONSUMER_SECRET_HEADER = "X-IC-ConsumerSecret";

        public static Guid GetInstantCheckoutConsumerKey(this IHeaderDictionary headers)
        {
            Guid appKey = Guid.Empty;

            if (headers.ContainsKey(INSTANT_CHECKOUT_CONSUMER_KEY_HEADER))
            {
                Guid.TryParse(headers[INSTANT_CHECKOUT_CONSUMER_KEY_HEADER], out appKey);
            }

            return appKey;
        }


        public static Guid GetInstantCheckoutConsumerSecret(this IHeaderDictionary headers)
        {
            Guid appKey = Guid.Empty;

            if (headers.ContainsKey(INSTANT_CHECKOUT_CONSUMER_SECRET_HEADER))
            {
                Guid.TryParse(headers[INSTANT_CHECKOUT_CONSUMER_SECRET_HEADER], out appKey);
            }

            return appKey;
        }
    }
}
