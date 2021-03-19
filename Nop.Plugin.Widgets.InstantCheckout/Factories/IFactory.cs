namespace Nop.Plugin.Widgets.InstantCheckout.Factories
{
    public interface IFactory<T>
    {
        T Initialize();
    }
}