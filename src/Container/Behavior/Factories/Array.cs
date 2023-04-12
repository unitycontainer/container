using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Builder;
using Unity.Extension;
using Unity.Policy;
using Unity.Resolution;
using Unity.Storage;

namespace Unity.Container
{
    internal static partial class Factories<TContext>
        where TContext : IBuilderContext
    {
        #region Fields

        private static MethodInfo? ArrayPipelineMethodInfo;
        
        private static Func<UnityContainer, Type, Type>? TargetTypeSelector;

        #endregion


        #region Factory

        public static object? Array(ref TContext context)
        {
            var type = context.Contract.Type;
            var pipeline = (ResolveDelegate<TContext>)UnityContainer.DummyPipeline;

            if (type.GetArrayRank() != 1)  // Verify array is valid
            {
                context.Error($"Invalid array {type}. Only arrays of rank 1 are supported");
                return UnityContainer.NoValue; 
            }

            var element = type.GetElementType()!;
            // TODO: TargetTypeSelector
            var target = (TargetTypeSelector ??= GetTargetTypeSelector(context.Policies))(context.Container, element!);

            var types = target.IsGenericType
                ? new Type[] { target, target.GetGenericTypeDefinition() }
                : new Type[] { target };

            pipeline = (ArrayPipelineMethodInfo ??= typeof(Factories<TContext>)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(ArrayPipeline))!)
                .CreatePipeline<TContext>(element, (MetadataFactory)GetMetadata);

            context.Policies.Set<ResolveDelegate<TContext>>(context.Type, pipeline);

            return pipeline!(ref context);


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
                            metadata = c.Container.Scope.ToArraySet(types);
                            manager.Data = new WeakReference(metadata);
                        }

                        if (!ReferenceEquals(c.Registration, manager))
                            manager.SetPipeline(pipeline);
                    }
                }

                return metadata;
            }

        }

        #endregion


        #region Implementation


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object? ArrayPipeline<TElement>(MetadataFactory factory, ref TContext context) 
            => Collection<TElement>(ref context, factory(ref context));


        /// <summary>
        /// Selects target Type during array resolution
        /// </summary>
        /// <param name="container">Container scope</param>
        /// <param name="element">Array element <see cref="Type"/></param>
        /// <returns><see cref="Type"/> of array's element</returns>
        private static Type ArrayTargetTypeSelector(UnityContainer container, Type element)
        {
            Type? next;
            Type? type = element;

            do
            {
                if (type.IsGenericType)
                {
                    if (container.Scope.Contains(type)) return type!;

                    var definition = type.GetGenericTypeDefinition();
                    if (container.Scope.Contains(definition)) return definition;

                    next = type.GenericTypeArguments[0]!;
                    if (container.Scope.Contains(next)) return next;
                }
                else if (type.IsArray)
                {
                    next = type.GetElementType()!;
                    if (container.Scope.Contains(next)) return next;
                }
                else
                {
                    return type!;
                }
            }
            while (null != (type = next));

            return element;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Func<UnityContainer, Type, Type> GetTargetTypeSelector(IPolicies policies) 
            => policies.CompareExchange<Array, Func<UnityContainer, Type, Type>>(ArrayTargetTypeSelector, null, (_, _, policy)
                => TargetTypeSelector = (Func<UnityContainer, Type, Type>)(policy ?? throw new ArgumentNullException(nameof(policy))))
                    ?? ArrayTargetTypeSelector;

        #endregion
    }
}
