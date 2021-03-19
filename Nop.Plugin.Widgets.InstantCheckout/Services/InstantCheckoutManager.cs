using Nop.Core;
using Nop.Services.Logging;

namespace Nop.Plugin.Widgets.InstantCheckout.Services
{
    /// <summary>
    /// Represents the Square payment manager
    /// </summary>
    public class InstantCheckoutManager
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly InstantCheckoutService _instantCheckoutService;
        private readonly InstantCheckoutSettings _instantCheckoutSettings;

        #endregion

        #region Ctor

        public InstantCheckoutManager(ILogger logger,
            IWorkContext workContext,
            IStoreContext storeContext,
            InstantCheckoutService instantCheckoutService,
            InstantCheckoutSettings instantCheckoutSettings)
        {
            _logger = logger;
            _workContext = workContext;
            _storeContext = storeContext;
            _instantCheckoutService = instantCheckoutService;
            _instantCheckoutSettings = instantCheckoutSettings;
        }

        #endregion


        #region Methods

        #region Common

        #endregion

        #region Shipping

        #endregion

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