using Nop.Core;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    /// <summary>
    /// Represents the Instant Checkout payment manager
    /// </summary>
    public class InstantCheckoutManager
    {
        #region Fields
        private readonly IStoreContext _storeContext;
        private readonly InstantCheckoutService _instantCheckoutService;

        #endregion

        #region Ctor

        public InstantCheckoutManager(IStoreContext storeContext,
            InstantCheckoutService instantCheckoutService)
        {
            _storeContext = storeContext;
            _instantCheckoutService = instantCheckoutService;
        }

        #endregion


        #region Methods

        #region Installation


        public void InstallWidgetPlugin(string baseUrl, string merchantId, string consumerKey, string consumerSecret)
        {
            var storeConfiguration = _storeContext.CurrentStore;

            _instantCheckoutService.InstallPluginOnInstantCheckout(baseUrl, merchantId, consumerKey, consumerSecret,
                storeConfiguration.Name, storeConfiguration.Url, NopVersion.CurrentVersion).Wait();
        }

        #endregion

        #endregion
    }
}