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
            if (null == context.Registration)
            {
                context.Registration = context.Container._scope.GetCache(in context.Contract);
            }

            if (null == context.Registration.Pipeline)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Registration)
                {
                    // TODO: threading

                    // Make sure it is still null and not created while waited for the lock
                    if (null == context.Registration.Pipeline)
                    {
                        context.Registration.Pipeline = Resolver;
                        context.Registration.Category = RegistrationCategory.Cache;
                    }
                }
            }

            return context.Registration.Pipeline(ref context)!;

            ///////////////////////////////////////////////////////////////////
            // Method
            object? Resolver(ref PipelineContext context)
            {
                var version = context.Container._scope.Version;
                var metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;

                if (null == metadata || version != metadata.Version())
                {
                    lock (context.Registration!)
                    {
                        metadata = (Metadata[]?)(context.Registration?.Data as WeakReference)?.Target;
                        if (null == metadata || version != metadata.Version())
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
                    array = new TElement[count];
                    count = 0;

                    // Resolve registered types
                    for (var i = array.Length; i > 0; i--)
                    {
                        var name = container._scope[in metadata[i]].Internal.Contract.Name;
                        var contract = new Contract(typeof(TElement), name);

                        try
                        {
                            array[count] = (TElement)container.Resolve(ref contract, ref context)!;
                            count += 1;
                        }
                        catch (ArgumentException ex) when (ex.InnerException is TypeLoadException)
                        {
                            // Ignore
                        }

                        if (context.IsFaulted) return RegistrationManager.NoValue;
                    }
                }
                else
                {
                    var error = new ErrorInfo();
                    var contract = new Contract(typeof(TElement), context.Contract.Name);
                    var childContext = context.CreateContext(ref contract, ref error);

                    // Nothing registered, try to resolve optional contract
                    try
                    {
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
                        // Ignore
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
