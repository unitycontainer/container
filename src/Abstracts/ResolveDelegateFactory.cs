using Unity.Builder;
using Unity.Resolution;

namespace Unity.Policy
{
    public delegate ResolveDelegate<BuilderContext> ResolveDelegateFactory(ref BuilderContext context);
}
