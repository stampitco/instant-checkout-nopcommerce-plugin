using System;
using Microsoft.AspNetCore.Http;

namespace Nop.Plugin.Widgets.InstantCheckout.HeaderExtensions
{
    public static class HttpHeaderDictionaryExtensions
    {
        internal const string InstantCheckoutConsumerKeyHeader = "X-IC-ConsumerKey";
        internal const string InstantCheckoutConsumerSecretHeader = "X-IC-ConsumerSecret";

        public static Guid GetInstantCheckoutConsumerKey(this IHeaderDictionary headers)
        {
            Guid appKey = Guid.Empty;

            if (headers.ContainsKey(InstantCheckoutConsumerKeyHeader))
            {
                Guid.TryParse(headers[InstantCheckoutConsumerKeyHeader], out appKey);
            }

            return appKey;
        }


        public static Guid GetInstantCheckoutConsumerSecret(this IHeaderDictionary headers)
        {
            Guid appKey = Guid.Empty;

            if (headers.ContainsKey(InstantCheckoutConsumerSecretHeader))
            {
                Guid.TryParse(headers[InstantCheckoutConsumerSecretHeader], out appKey);
            }

            return appKey;
        }
    }
}
