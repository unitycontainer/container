using Unity.Injection;

namespace Unity.Container
{
    // TODO: Create more understandable type

    public delegate TReturn ImportProvider<TInfo, TReturn>(ref TInfo info)
        where TInfo : IInjectionInfo;

    public delegate void ImportDataProvider<TInfo>(ref TInfo info, object? value)
        where TInfo : IInjectionInfo;
}
