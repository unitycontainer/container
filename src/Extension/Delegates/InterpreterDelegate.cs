using Unity.Builder;
using Unity.Injection;

namespace Unity.Extension
{
    // TODO: ???
    public delegate object? InterpreterDelegate<TContext, TDescriptor, TMemberInfo>(ref TContext context, ref TDescriptor import, object? data)
        where TDescriptor : IInjectionInfo<TMemberInfo>
        where TContext    : IBuilderContext;
}

