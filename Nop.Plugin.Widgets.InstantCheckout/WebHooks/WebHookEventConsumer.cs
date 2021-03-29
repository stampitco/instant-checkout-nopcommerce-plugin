using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.InstantCheckout.Constants;
using Nop.Plugin.Widgets.InstantCheckout.DTOs.Orders;
using Nop.Plugin.Widgets.InstantCheckout.Helpers;
using Nop.Plugin.Widgets.InstantCheckout.Services;
using Nop.Services.Events;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.InstantCheckout.WebHooks
{
    public class WebHookEventConsumer : IConsumer<EntityUpdatedEvent<Order>>
    {
        private IDTOHelper _dtoHelper;
        private readonly InstantCheckoutService _instantCheckoutService;
        private readonly InstantCheckoutSettings _instantCheckoutSettings;


        public WebHookEventConsumer(IStoreService storeService, InstantCheckoutService instantCheckoutService,
            InstantCheckoutSettings instantCheckoutSettings)
        {
            _dtoHelper = EngineContext.Current.Resolve<IDTOHelper>();
            _instantCheckoutService = instantCheckoutService;
            _instantCheckoutSettings = instantCheckoutSettings;
        }

        public void HandleEvent(EntityUpdatedEvent<Order> eventMessage)
        {
            OrderDto orderDto = _dtoHelper.PrepareOrderDTO(eventMessage.Entity);

            if (orderDto.Deleted == true)
            {
                _instantCheckoutService.InstantCheckoutWebHooks(_instantCheckoutSettings.InstantCheckoutApiBaseUrl,
                    _instantCheckoutSettings.MerchantId.ToString(), WebHookNames.ORDERS_DELETE, orderDto).Wait();
            }

            if (orderDto.OrderStatus == "Complete")
            {
                _instantCheckoutService.InstantCheckoutWebHooks(_instantCheckoutSettings.InstantCheckoutApiBaseUrl,
                    _instantCheckoutSettings.MerchantId.ToString(), WebHookNames.ORDERS_UPDATE, orderDto).Wait();
            }
            else if (orderDto.OrderStatus == "Cancelled")
            {
                _instantCheckoutService.InstantCheckoutWebHooks(_instantCheckoutSettings.InstantCheckoutApiBaseUrl,
                    _instantCheckoutSettings.MerchantId.ToString(), WebHookNames.ORDERS_UPDATE, orderDto).Wait();
            }
        }
    }
}
