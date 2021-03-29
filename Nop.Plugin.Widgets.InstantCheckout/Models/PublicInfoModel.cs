using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.InstantCheckout.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public Guid MerchantId { get; set; }
        public string InstantCheckoutBaseUrl { get; set; }
        public int ProductId { get; set; }
        public string Products { get; set; }
        public bool IsCart { get; set; }
    }

    public class ProductQuantity
    {
        public int i { get; set; }
        public int q { get; set; }
    }
}