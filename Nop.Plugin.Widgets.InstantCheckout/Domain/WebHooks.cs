using System;
using Nop.Core;

namespace Nop.Plugin.Widgets.InstantCheckout.Domain
{
    public class WebHooks : BaseEntity
    {
        public string User { get; set; }

        public string Id { get; set; }

        public string ProtectedData { get; set; }

        public Byte[] RowVer { get; set; }
    }
}
