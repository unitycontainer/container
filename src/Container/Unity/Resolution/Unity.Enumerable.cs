using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static readonly MethodInfo? EnumerableFactoryMethod;

        #endregion


        #region Unregistered

        private ResolveDelegate<PipelineContext> ResolveUnregisteredEnumerable(Type type)
        {
            if (!_policies.TryGet(type, out ResolveDelegate<PipelineContext>? pipeline))
            {
                var target = type.GenericTypeArguments[0];

                var types = target.IsGenericType
                    ? new[] { target, target.GetGenericTypeDefinition() }
                    : new[] { target };

                pipeline = _policies.AddOrGet(type, EnumerableFactoryMethod!.CreatePipeline(target, types));
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
                context.Registration = context.Container._scope.GetCache(in context.Contract);
            }

            // Get Pipeline
            if (context.Registration.Pipeline is null)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // TODO: threading

                    // Make sure it is still null and not created while waited for the lock
                    if (context.Registration.Pipeline is null)
                    {
                        context.Registration.Pipeline = Resolver;
                        context.Registration.Category = RegistrationCategory.Cache;
                    }
                }
            }

            // Execute
            return context.Registration.Pipeline(ref context)!;

            ///////////////////////////////////////////////////////////////////
            // Method
            object? Resolver(ref PipelineContext context)
            {
                var version = context.Container._scope.Version;
                var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;

                if (metadata is null || version != metadata.Version())
                {
                    lock (context.Registration!)
                    {
                        metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
                        if (metadata is null || version != metadata.Version())
                        {
                            metadata = context.Defaults.MetaEnumeration(context.Container._scope, types);
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

                        var name = container._scope[in metadata[i]].Internal.Contract.Name;
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
                                return RegistrationManager.NoValue;
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
