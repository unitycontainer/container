using Unity.Builder;
using Unity.Resolution;

namespace Unity.Policy
{
    public delegate ResolveDelegate<BuilderContext> ResolveDelegateFactory(ref BuilderContext context);

    public interface IResolveDelegateFactory
    {
        ResolveDelegate<BuilderContext> GetResolver(ref BuilderContext context);
    }
}
