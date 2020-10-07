using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {

        private static object? ResolveUnregistered(ref Contract contract, ref PipelineContext parent)
        {
            return parent.Error("NotImplementedException");
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
            throw new NotImplementedException();

            //var info = new RequestInfo(overrides);
            //var context = new PipelineContext(ref info, ref contract, this);

            //return _policies.ResolveUnregistered(ref context);
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
        private object? ResolveUnregisteredGeneric(ref Contract contract, ref Contract generic, ref PipelineContext parent)
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

    }
}
