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
                                 .GetDeclaredMethod(nameof(EnumeratorPipelineFactory))!;
        #endregion


        #region Setup

        public static void Setup(ExtensionContext context)
        {
            _policies = (Policies)context.Policies;
            _policies.Set<FromTypeFactory<PipelineContext>>(typeof(IEnumerable<>), ResolveUnregisteredEnumerable);
        }

        #endregion


        #region Unregistered

        private static ResolveDelegate<PipelineContext> ResolveUnregisteredEnumerable(Type type)
        {
            if (!_policies!.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                var target = type.GenericTypeArguments[0];

                var types = target.IsGenericType
                    ? new[] { target, target.GetGenericTypeDefinition() }
                    : new[] { target };

                pipeline = _method!.CreatePipeline(target, types);
                _policies.Set<ResolveDelegate<PipelineContext>>(type, pipeline);
            }

            return pipeline!;
        }

        #endregion


        #region Factory

        private static object? EnumeratorPipelineFactory<TElement>(Type[] types, ref PipelineContext context)
        {
            // Get Registration
            if (context.Registration is null)
            {
                context.Registration = context.Container.Scope.GetCache(in context.Contract);
            }

            // Get Pipeline
            var pipeline = context.Registration.GetPipeline(context.Container.Scope);
            if (pipeline is null)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // TODO: threading

                    // Make sure it is still null and not created while waited for the lock
                    pipeline = context.Registration.GetPipeline(context.Container.Scope);
                    if (pipeline is null)
                    {
                        pipeline = context.Registration.SetPipeline(Resolver, context.Container.Scope);
                        context.Registration.Category = RegistrationCategory.Cache;
                    }
                }
            }

            // Execute
            return pipeline(ref context)!;

            ///////////////////////////////////////////////////////////////////
            // Method
            object? Resolver(ref PipelineContext context)
            {
                var version = context.Container.Scope.Version;
                var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;

                if (metadata is null || version != metadata.Version())
                {
                    lock (context.Registration!)
                    {
                        metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
                        if (metadata is null || version != metadata.Version())
                        {
                            metadata = context.Container.Scope.ToEnumerableSet(types);
                            context.Registration!.Data = new WeakReference(metadata);
                        }
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
        }

        #endregion

    }
}
