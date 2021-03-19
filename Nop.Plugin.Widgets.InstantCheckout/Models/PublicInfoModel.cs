using System;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.InstantCheckout.Models
{
    public class PublicInfoModel : BaseNopModel
    {
        public Guid MerchantId { get; set; }
        public string InstantCheckoutBaseUrl { get; set; }
        public int ProductId { get; set; }
    }
}