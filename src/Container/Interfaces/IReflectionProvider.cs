
namespace Unity.Container
{
    public interface IReflectionProvider<TElement>
    {
        ReflectionInfo<TElement> GetInfo(TElement element);
    }
}
