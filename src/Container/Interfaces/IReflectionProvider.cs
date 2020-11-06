
namespace Unity.Container
{
    public interface IReflectionProvider<TElement>
    {
        ImportType FillReflectionInfo(ref ReflectionInfo<TElement> reflectionInfo);
    }
}
