using System;
using Unity.Builder;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public delegate ResolveDelegate<BuilderContext> TypeFactoryDelegate(Type type, PolicySet policies);
}
