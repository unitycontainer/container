using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Fields

        private static ResolverOverride[] _emptyOverrides;

        #endregion


        #region Throwing/Silent

        /// <summary>
        /// Resolve unknown type throwing exception in case of an error
        /// </summary>
        private object? ResolveUnregistered(ref Contract contract, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var value = ResolveUnregistered(ref contract, ref request);

            // TODO: Check synchronized disposal

            if (request.IsFaulted)
            {
                // Throw if user exception
                request.ErrorInfo.Throw();

                // Throw ResolutionFailedException otherwise
                throw new ResolutionFailedException(in contract, request.ErrorInfo.Message);
            }

            return value;
        }

        /// <summary>
        /// Silently resolve unknown type
        /// </summary>
        private object? ResolveUnregistered(ref Contract contract)
        {
            var request = new RequestInfo(_emptyOverrides ??= new ResolverOverride[0]);
            var value = ResolveUnregistered(ref contract, ref request);

            return request.IsFaulted ? null : value;
        }

        #endregion

        
        #region Implementation

        /// <summary>
        /// Resolve unregistered from Contract
        /// </summary>
        private object? ResolveUnregistered(ref Contract contract, ref RequestInfo request)
        {
            PipelineContext context;
            RegistrationManager? manager;
            Contract generic = default;

            // Skip to parent if non generic
            if (contract.Type.IsGenericType)
            {
                // Fill the Generic Type Definition
                generic = contract.With(contract.Type.GetGenericTypeDefinition());

                // Check if generic factory is registered
                if (null != (manager = _scope.GetBoundGeneric(in contract, in generic)))
                {
                    context = new PipelineContext(this, ref contract, manager, ref request);
                    return GenericRegistration(generic.Type!, ref context);
                }
            }

            var container = this;
            while (null != (container = container.Parent!))
            {
                // Try to get registration
                manager = container._scope.Get(in contract);
                if (null != manager)
                {
                    var value = Unsafe.As<LifetimeManager>(manager).GetValue(_scope);
                    if (value.IsValue()) return value;

                    context = new PipelineContext(container, ref contract, manager, ref request);

                    return ImportSource.Local == manager.Source
                        ? ResolveRegistered(ref context)
                        : container.ResolveRegistered(ref context);
                }

                // Skip to parent if non generic
                if (!contract.Type.IsGenericType) continue;

                // Check if generic factory is registered
                if (null != (manager = container._scope.GetBoundGeneric(in contract, in generic)))
                {
                    context = new PipelineContext(container, ref contract, manager, ref request);

                    return ImportSource.Local == manager.Source
                        ? GenericRegistration(generic.Type!, ref context)
                        : container.GenericRegistration(generic.Type!, ref context);
                }
            }

            context = new PipelineContext(this, ref contract, ref request);

            return contract.Type.IsGenericType
                ? GenericUnregistered(ref generic, ref context)
                : contract.Type.IsArray
                    ? ResolveUnregisteredArray(ref context)
                    : ResolveUnregistered(ref context);
        }

        #endregion
    }
}
