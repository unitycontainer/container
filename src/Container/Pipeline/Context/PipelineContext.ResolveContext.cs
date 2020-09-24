using System;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Container
{
    public partial struct PipelineContext : IResolveContext
    {
        public readonly Type Type
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Type;
                }
            }
        }

        public string? Name
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<Contract>(_contract.ToPointer()).Name;
                }
            }
        }

        public readonly ResolverOverride[] Overrides 
        {
            get
            {
                unsafe
                {
                    return Unsafe.AsRef<RequestInfo>(_request.ToPointer()).Overrides;
                }
            }
        }

        public object? Resolve(Type type, string? name)
        {
            throw new NotImplementedException();
            //// Process overrides if any
            //if (0 < Overrides.Length)
            //{
            //    NamedType namedType = new NamedType
            //    {
            //        Type = type,
            //        Name = name
            //    };

            //    // Check if this parameter is overridden
            //    for (var index = Overrides.Length - 1; index >= 0; --index)
            //    {
            //        var resolverOverride = Overrides[index];
            //        // If matches with current parameter
            //        if (resolverOverride is IResolve resolverPolicy &&
            //            resolverOverride is IEquatable<NamedType> comparer && comparer.Equals(namedType))
            //        {
            //            var context = this;

            //            return DependencyResolvePipeline(ref context, resolverPolicy.Resolve);
            //        }
            //    }
            //}

            //var pipeline = ContainerContext.Container.GetPipeline(type, name);
            //LifetimeManager? manager = pipeline.Target as LifetimeManager;

            //// Check if already created and acquire a lock if not
            //if (manager is PerResolveLifetimeManager)
            //{
            //    manager = List.Get(type, name, typeof(LifetimeManager)) as LifetimeManager ?? manager;
            //}

            //if (null != manager)
            //{
            //    // Make blocking check for result
            //    var value = manager.Get(LifetimeContainer);
            //    if (LifetimeManager.NoValue != value) return value;

            //    if (null != manager.Scope)
            //    {
            //        var scope = ContainerContext;
            //        try
            //        {
            //            ContainerContext = (ContainerContext)manager.Scope;
            //            return Resolve(type, name, pipeline);
            //        }
            //        finally
            //        {
            //            ContainerContext = scope;
            //        }
            //    }
            //}

            //return Resolve(type, name, pipeline);
        }
    }
}
