using System;
using Unity.Builder;
using Unity.Resolution;

namespace Unity
{
    public delegate ResolveDelegate<BuilderContext> TypeFactoryDelegate(Type type, UnityContainer container);
}
