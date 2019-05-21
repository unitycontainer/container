using System;
using Unity.Builder;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    public delegate ResolveDelegate<BuilderContext> TypeFactoryDelegate(Type type, IRegistration policies);
}
