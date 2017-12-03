namespace Unity.Container
{
    public interface IContainerContext : IPolicyRegistry
    {
        IUnityContainer Container { get; }
    }
}
