
namespace Unity.Container
{
    public delegate TReturn ImportProvider<TInfo, TReturn>(ref TInfo info);

    public delegate TReturn ImportDataProvider<TInfo, TReturn>(ref TInfo info, object? value);
}
