using System;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Registered Contract

        private object? ResolveRegistration(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var info = new RequestInfo(overrides);
            var context = new ResolutionContext(this, ref contract, manager, ref info);


            // Check if pipeline has been created already
            if (null == context.Manager!.Pipeline)
            {
                // Lock the Manager to prevent creating pipeline multiple times2
                lock (context.Manager)
                {
                    // Make sure it is still null and not created while waited for the lock
                    if (null == context.Manager.Pipeline)
                    {
                        context.Manager!.Pipeline = _policies.BuildPipeline(ref context);
                    }
                }
            }

            // Resolve
            return context.Manager!.Pipeline(ref context);

            //return context.IsFaulted
            //    ? throw task.AsTask().Exception! // TODO: Proper error reporting
            //    : task.Result;
        }


        private object? GenericRegistration(ref Contract contract, RegistrationManager manager, ResolverOverride[] overrides)
        {
            var info = new RequestInfo(overrides);
            var context = new ResolutionContext(this, ref contract, manager, ref info);
            var factory = (RegistrationManager)manager.Data!;

            // Calculate new Type
            manager.Category = RegistrationCategory.Type;
            manager.Data = factory.Type?.MakeGenericType(contract.Type.GenericTypeArguments);

            // If any injection members are present, build is required
            if (manager.RequireBuild) return _policies.ResolveContract(ref context);

            // No injectors, redirect
            return _policies.ResolveMapped(ref context);
        }


        private static ValueTask<object?> ResolveRegistration(ref ResolutionContext context)
        {
            throw new NotImplementedException();

            //// Check if pipeline has been created already
            //if (null == context.Manager!.ResolveDelegate)
            //{
            //    // Prevent creating pipeline multiple times
            //    lock (context.Manager)
            //    {
            //        // Make sure it is still not created while waited for the lock
            //        if (null == context.Manager.ResolveDelegate)
            //        {
            //            var lifetime = (LifetimeManager)context.Manager;

            //            //context.Manager.ResolveDelegate = context.Container._policies.TypePipeline;
                        
            //            // TODO: implement
            //            //context.Manager.ResolveDelegate = context.Manager.Category switch
            //            //{
            //            //    RegistrationCategory.Instance => context.Container._policies.InstancePipeline,
            //            //    RegistrationCategory.Factory  => context.Container._policies.FactoryPipeline,

            //            //    RegistrationCategory.Clone when ResolutionStyle.OnceInLifetime == lifetime.Style => context.Container._policies.TypePipeline,
            //            //    RegistrationCategory.Clone when ResolutionStyle.OnceInWhile    == lifetime.Style => context.Container._policies.TypePipeline,
            //            //    RegistrationCategory.Clone when ResolutionStyle.EveryTime      == lifetime.Style => context.Container._policies.TypePipeline,

            //            //    RegistrationCategory.Type when ResolutionStyle.OnceInLifetime == lifetime.Style 
            //            //        => context.Container._policies.TypePipeline,

            //            //    RegistrationCategory.Type when ResolutionStyle.OnceInWhile == lifetime.Style 
            //            //        => context.Container._policies.BalancedPipelineFactory(ref context),

            //            //    RegistrationCategory.Type when ResolutionStyle.EveryTime == lifetime.Style 
            //            //        => context.Container._policies.OptimizedPipelineFactory(ref context),

            //            //    _ => throw new InvalidOperationException($"Registration {context.Type}/{context.Name} has unsupported category {context.Manager.Category}")
            //            //};
            //        }
            //    }
            //}

            //var value = ((ResolveDelegate<ResolutionContext>)context.Manager!.ResolveDelegate)(ref context);

            //// Resolve in current context
            //return new ValueTask<object?>(value);
        }

        #endregion


        #region Unregistered

        private static object? ResolveUnregistered(ref ResolutionContext context)
        {
            return null;
            //throw new NotImplementedException();
            //// Check if resolver already exist
            //var resolver = _policies[contract.Type];

            //// Nothing found, requires build
            //if (null == resolver)
            //{
            //    // Build new and try to save it
            //    resolver = _policies.UnregisteredPipelineFactory(in contract);
            //    resolver = _policies.GetOrAdd(contract.Type, resolver);
            //}

            //var context = new ResolveContext(this, in contract, overrides);
            //return resolver(ref context);
        }

        /// <summary>
        /// Resolve unregistered <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// Although <see cref="Contract"/> is used as an input, but only <see cref="Type"/> is
        /// used to identify correct entry.
        /// </remarks>
        /// <param name="contract"><see cref="Contract"/> to use for resolution</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested object</returns>
        private object? ResolveUnregistered(ref Contract contract, ResolverOverride[] overrides)
        {
            var info = new RequestInfo(overrides);
            var context = new ResolutionContext(ref info, ref contract, this);

            return _policies.ResolveUnregistered(ref context);
        }

        /// <summary>
        /// Resolve unregistered generic <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// Although <see cref="Contract"/> is used as an input, but only <see cref="Type"/> is
        /// used to identify correct entry.
        /// This method will first look for a type factory, before invoking default resolver factory
        /// </remarks>
        /// <param name="contract"><see cref="Contract"/> to use for resolution</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested object</returns>
        private object? ResolveUnregisteredGeneric(ref Contract contract, ref Contract generic, ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
            //var context = new ResolveContext(this, in contract, overrides);

            //// Check if resolver already exist
            //var resolver = _policies[contract.Type];
            //if (null != resolver) return resolver(ref context);

            //var factory = _policies.Get<ResolveDelegateFactory>(generic.Type);
            //if (null != factory)
            //{
            //    // Build from factory and try to store it
            //    resolver = factory(in contract);
            //    resolver = _policies.GetOrAdd(contract.Type, resolver);
            //    return resolver(ref context);
            //}

            //// Build new and try to save it
            //resolver = _policies.UnregisteredPipelineFactory(in contract);
            //resolver = _policies.GetOrAdd(contract.Type, resolver);

            //// Resolve
            //return resolver(ref context);
        }

        #endregion


        #region Array

        private static object? ResolveArray(ref ResolutionContext context)
        {
            throw new NotImplementedException();
            //var context = new ResolveContext(this, in contract, overrides);
            //var resolver = _policies[contract.Type];

            //// Nothing found, requires build
            //if (null == resolver)
            //{
            //    resolver = (ref ResolveContext c) => c.Existing;
            //    _policies[contract.Type] = resolver;
            //}

            //return resolver(ref context);
        }

        /// <summary>
        /// Resolve array
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> the array factory will be stored at</param>
        /// <param name="overrides">Overrides to use during resolution</param>
        /// <exception cref="ResolutionFailedException">if anything goes wrong</exception>
        /// <returns>Requested array</returns>
        private object? ResolveArray(ref Contract contract, ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
            //var context = new ResolutionContext(this, in contract, overrides);
            //var resolver = _policies[contract.Type];

            //// Nothing found, requires build
            //if (null == resolver)
            //{
            //    resolver = (ref ResolutionContext c) => c.Existing;
            //    _policies[contract.Type] = resolver;
            //}

            //return resolver(ref context);
        }

        #endregion


        #region Resolve Async

        /// <summary>
        /// Builds and resolves registered contract
        /// </summary>
        /// <param name="state"><see cref="ResolveContractAsyncState"/> objects holding 
        /// resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveContractAsync(object? state)
        {
            RequestInfoAsync context = (RequestInfoAsync)state!;



            return Task.FromResult<object?>(context.Manager);
        }

        /// <summary>
        /// Builds and resolves unregistered <see cref="Type"/>
        /// </summary>
        /// <param name="state"><see cref="RequestInfoAsync"/> objects holding resolution request data</param>
        /// <returns>Resolved object or <see cref="Task.FromException(System.Exception)"/> if failed</returns>
        private Task<object?> ResolveAsync(object? state)
        {
            RequestInfoAsync context = (RequestInfoAsync)state!;



            return Task.FromResult<object?>(context.Contract.Type);
        }

        #endregion
    }
}
