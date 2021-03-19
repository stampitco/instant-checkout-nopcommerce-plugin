namespace Nop.Plugin.Widgets.InstantCheckout.JSON.Serializers
{
    using Nop.Plugin.Widgets.InstantCheckout.DTOs;

    public interface IJsonFieldsSerializer
    {
        string Serialize(ISerializableObject objectToSerialize, string fields);
    }
}
