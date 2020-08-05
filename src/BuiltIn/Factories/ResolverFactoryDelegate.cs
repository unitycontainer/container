using System;
using Unity.Pipeline;
using Unity.Resolution;

namespace Unity
{
    public delegate ResolveDelegate<ResolveContext> ResolverFactoryDelegate(Type type);
}
