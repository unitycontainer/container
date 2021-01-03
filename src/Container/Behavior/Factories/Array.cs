using System;
using System.Reflection;
using Unity.Extension;
using Unity.Storage;

namespace Unity.Container
{
    internal static partial class UnityDefaultBehaviorExtension<TContext>
    {
        #region Fields

        private static MethodInfo? ArrayPipelineMethodInfo;
        
        private static SelectorDelegate<Type, Type>? TargetTypeSelector;

        #endregion


        #region Factory

        public static object? ArrayFactory(ref TContext context)
        {
            var type = context.Contract.Type;

            if (type.GetArrayRank() != 1)  // Verify array is valid
                return context.Error($"Invalid array {type}. Only arrays of rank 1 are supported");

            var element = type.GetElementType()!;
            var target = (TargetTypeSelector ??= GetTargetTypeSelector(context.Policies))(context.Container, element!);
            var state = target.IsGenericType
                ? new State(target, target.GetGenericTypeDefinition())
                : new State(target);

            state.Pipeline = (ArrayPipelineMethodInfo ??= typeof(UnityDefaultBehaviorExtension<TContext>)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(ArrayPipeline))!)
                .CreatePipeline<TContext>(element, state);

            context.Policies.Set<ResolveDelegate<TContext>>(context.Type, state.Pipeline);

            return state.Pipeline!(ref context);
        }

        #endregion


        #region Implementation

        private static object? ArrayPipeline<TElement>(State state, ref TContext context)
        {
            var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
            if (metadata is null || context.Container.Scope.Version != metadata.Version())
            {
                var manager = context.Container.Scope.GetCache(in context.Contract,
                    () => new InternalLifetimeManager(RegistrationCategory.Cache));

                lock (manager)
                {
                    metadata = (Metadata[]?)(manager.Data as WeakReference)?.Target;
                    if (metadata is null || context.Container.Scope.Version != metadata.Version())
                    {
                        metadata = context.Container.Scope.ToArraySet(state.Types);
                        manager.Data = new WeakReference(metadata);
                    }

                    if (!ReferenceEquals(context.Registration, manager))
                        manager.SetPipeline(context.Container.Scope, state.Pipeline);
                }
            }

            var count = 0;
            var array = new TElement[metadata.Count()];
            var container = context.Container;
            var hash = typeof(TElement).GetHashCode();

            ErrorInfo errorInfo = default;
            Contract contract = default;

            var local = context.CreateContext(ref contract, ref errorInfo);

            for (var i = array.Length; i > 0; i--)
            {
                local.Reset();

                var name = container.Scope[in metadata[i]].Internal.Contract.Name;
                contract = new Contract(Contract.GetHashCode(hash, name!.GetHashCode()), typeof(TElement), name);

                var value = container.Resolve(ref local);

                if (errorInfo.IsFaulted)
                {
                    if (errorInfo.Exception is ArgumentException ex && ex.InnerException is TypeLoadException)
                    {
                        continue; // Ignore
                    }
                    else
                    {
                        context.ErrorInfo = errorInfo;
                        return UnityContainer.NoValue;
                    }
                }

                array[count++] = (TElement)value!;
            }

            if (count < array.Length) Array.Resize(ref array, count);

            context.Target = array;

            return array;
        }

        private static SelectorDelegate<Type, Type> GetTargetTypeSelector(IPolicies policies)
        {
            return policies.CompareExchange<Array, SelectorDelegate<Type, Type>>(ArrayTargetTypeSelector, null, (_, _, policy)
                => TargetTypeSelector = (SelectorDelegate<Type, Type>)(policy ?? throw new ArgumentNullException(nameof(policy))))
                ?? ArrayTargetTypeSelector;
        }

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

        #endregion
    }
}
