using Nop.Plugin.Widgets.InstantCheckout.ModelBinders;

namespace Nop.Plugin.Widgets.InstantCheckout.Models.OrdersParameters
{
    using Microsoft.AspNetCore.Mvc;

    [ModelBinder(typeof(ParametersModelBinder<OrdersCountParametersModel>))]
    public class OrdersCountParametersModel : BaseOrdersParametersModel
    {
        // Nothing special here, created just for clarity.
    }
}