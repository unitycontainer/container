using System;
using Unity;
using Unity.Resolution;

namespace Unity
{
    public delegate ResolveDelegate<PipelineContext> TypeFactoryDelegate(Type type, UnityContainer container);
}
