using System;
using System.Reflection;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    internal static partial class Factories<TContext>
    {
        #region Fields

        private static MethodInfo? EnumerablePipelineMethodInfo;

        #endregion


        #region Factory

        public static ResolveDelegate<TContext> Enumerable(ref TContext context)
        {
            var pipeline = (ResolveDelegate<TContext>)UnityContainer.DummyPipeline;
            var target = context.Type.GenericTypeArguments[0];
            
            var types = target.IsGenericType
                ? new Type[] { target, target.GetGenericTypeDefinition() }
                : new Type[] { target };

            pipeline = (EnumerablePipelineMethodInfo ??= typeof(Factories<TContext>)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(EnumerablePipeline))!)
                .CreatePipeline<TContext>(target, (MetadataFactory)GetMetadata);

            // TODO: context.Policies.Set<ResolveDelegate<TContext>>(context.Type, pipeline);

            return pipeline;

            // Metadata Factory
            Metadata[] GetMetadata(ref TContext c)
            {
                var metadata = (Metadata[]?)(c.Registration?.Data as WeakReference)?.Target;
                if (metadata is null || c.Container.Scope.Version != metadata.Version())
                {
                    var manager = c.Container.Scope.GetCache(in c.Contract,
                        () => new InternalLifetimeManager(RegistrationCategory.Cache));

                    lock (manager)
                    {
                        metadata = (Metadata[]?)(manager.Data as WeakReference)?.Target;
                        if (metadata is null || c.Container.Scope.Version != metadata.Version())
                        {
                            metadata = c.Container.Scope.ToEnumerableSet(types);
                            manager.Data = new WeakReference(metadata);
                        }

                        if (!ReferenceEquals(c.Registration, manager))
                            manager.SetPipeline(c.Container.Scope, pipeline);
                    }
                }

                return metadata;
            }
        }

        #endregion


        #region Implementation

        private static object? EnumerablePipeline<TElement>(MetadataFactory factory, ref TContext context)
        {
            var metadata = factory(ref context);
            var count = metadata.Count();

            if (0 < count) return Collection<TElement>(ref context, metadata);

            // Nothing is registered, try to resolve optional contract

            TElement[] array;
            var typeHash = typeof(TElement).GetHashCode();

            try
            {
                var name = context.Contract.Name;
                var hash = Contract.GetHashCode(typeHash, name?.GetHashCode() ?? 0);
                var error = new ErrorInfo();
                var contract = new Contract(hash, typeof(TElement), name);
                var value = context.FromContract(ref contract, ref error);

                if (error.IsFaulted)
                {
#if NET45
                    array = new TElement[0];
#else
                    array = System.Array.Empty<TElement>();
#endif
                }
                else
                {
                    count = 1;
                    array = new TElement[] { (TElement)value! };
                };
            }
            catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
            {
#if NET45
                array = new TElement[0];
#else
                array = System.Array.Empty<TElement>();
#endif
            }

            if (count < array.Length) System.Array.Resize(ref array, count);

            // TODO: PerResolve
            context.PerResolve = array;

            return array;
        }

        #endregion
    }
}
