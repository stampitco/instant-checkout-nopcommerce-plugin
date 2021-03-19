using System;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.InstantCheckout.Maps
{
    public interface IJsonPropertyMapper
    {
        Dictionary<string, Tuple<string, Type>> GetMap(Type type);
    }
}