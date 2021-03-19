using System.Collections.Generic;

namespace Nop.Plugin.Widgets.InstantCheckout.Models.ShippingsParameters
{
    public partial class ShippingOptionsViewModel
    {
        public ShippingOptionsViewModel()
        {
            ShippingOptions = new List<ShippingOptionModel>();
            Warnings = new List<string>();
        }

        public IList<ShippingOptionModel> ShippingOptions { get; set; }

        public IList<string> Warnings { get; set; }

        #region Nested Classes

        public partial class ShippingOptionModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public decimal Price { get; set; }

            public string Plugin { get; set; }
        }

        #endregion
    }
}
