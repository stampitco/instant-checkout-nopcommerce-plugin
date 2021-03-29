namespace Nop.Plugin.Widgets.InstantCheckout.Constants
{
    public class Configurations
    {
        public const int DEFAULT_ACCESS_TOKEN_EXPIRATION = 315360000;
        public const int DEFAULT_REFRESH_TOKEN_EXPIRATION = int.MaxValue;
        public const int DEFAULT_LIMIT = 50;
        public const int DEFAULT_PAGE_VALUE = 1;
        public const int DEFAULT_SINCE_ID = 0;
        public const int DEFAULT_CUSTOMER_ID = 0;
        public const string DEFAULT_ORDER = "Id";
        public const int MAX_LIMIT = 250;
        public const int MIN_LIMIT = 1;
        public const string PUBLISHED_STATUS = "published";
        public const string UNPUBLISHED_STATUS = "unpublished";
        public const string ANY_STATUS = "any";
        public const string JSON_TYPE_MAPS_PATTERN = "json.maps";
    }
}