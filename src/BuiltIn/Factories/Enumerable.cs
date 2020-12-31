using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Container;
using Unity.Extension;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public static class EnumFactory
    {
        #region Fields

        private static Policies? _policies;
        private static readonly MethodInfo _method 
            = typeof(EnumFactory).GetTypeInfo()
                                 .GetDeclaredMethod(nameof(EnumeratorPipeline))!;
        #endregion


        #region Setup

        public static void Setup(ExtensionContext context)
        {
            _policies = (Policies)context.Policies;
            _policies.Set<FromTypeFactory<PipelineContext>>(typeof(IEnumerable<>), EnumerableFactory);
        }

        #endregion


        #region Unregistered

        private static ResolveDelegate<PipelineContext> EnumerableFactory(Type type)
        {
            if (_policies!.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
                return pipeline!;

            var target = type.GenericTypeArguments[0];
            var state = target.IsGenericType
                ? new State(target, target.GetGenericTypeDefinition())
                : new State(target);


            state.Pipeline = _method!.CreatePipeline(target, state);
            _policies.Set<ResolveDelegate<PipelineContext>>(type, state.Pipeline);

            return state.Pipeline;
        }

        #endregion


        #region Factory

        private static object? EnumeratorPipeline<TElement>(State state, ref PipelineContext context)
        {
            var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
            if (metadata is null || context.Container.Scope.Version != metadata.Version())
            {
                var manager = context.Container.Scope.GetCache(in context.Contract);

                lock (manager)
                {
                    metadata = (Metadata[]?)(manager.Data as WeakReference)?.Target;
                    if (metadata is null || context.Container.Scope.Version != metadata.Version())
                    {
                        metadata = context.Container.Scope.ToEnumerableSet(state.Types);
                        manager.Data = new WeakReference(metadata);
                    }

                    if (!ReferenceEquals(context.Registration, manager))
                        manager.SetPipeline(context.Container.Scope, state.Pipeline);
                }
            }

            TElement[] array;
            var count = metadata.Count();

            if (0 < count)
            {
                var container = context.Container;
                var hash = typeof(TElement).GetHashCode();

                array = new TElement[count];
                count = 0;

                ErrorInfo errorInfo = default;
                Contract contract = default;

                var local = context.CreateContext(ref contract, ref errorInfo);

                for (var i = array.Length; i > 0; i--)
                {
                    local.Reset();

                    var name = container.Scope[in metadata[i]].Internal.Contract.Name;
                    contract = new Contract(Contract.GetHashCode(hash, name?.GetHashCode() ?? 0), typeof(TElement), name);

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
            }
            else
            {
                var error = new ErrorInfo();
                var contract = new Contract(typeof(TElement), context.Contract.Name);
                var childContext = context.CreateContext(ref contract, ref error);

                try
                {
                    // Nothing is registered, try to resolve optional contract
                    childContext.Target = context.Container.Resolve(ref childContext)!;
                    if (childContext.IsFaulted)
                    {
                        array = new TElement[0];
                    }
                    else
                    {
                        count = 1;
                        array = new TElement[] { (TElement)childContext.Target! };
                    };
                }
                catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                {
                    array = new TElement[0];
                }
            }

            if (count < array.Length) Array.Resize(ref array, count);

            context.Target = array;

            return array;
        }

        #endregion



        #region Nested State

        private class State
        {
            public readonly Type[] Types;
            public ResolveDelegate<PipelineContext>? Pipeline;
            public State(params Type[] types) => Types = types;

        }

        #endregion
    }
}
