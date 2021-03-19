using System;

namespace Nop.Plugin.Widgets.InstantCheckout.DTOs
{
    public interface ISerializableObject
    {
        string GetPrimaryPropertyName();
        Type GetPrimaryPropertyType();
    }
}