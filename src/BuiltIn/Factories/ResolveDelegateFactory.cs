using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class DelegateFactory
    {
        public static ResolveDelegate<ResolveContext> Factory(ref ResolveContext context)
        {
            return (ref ResolveContext c) => null;
        }
    }
}
