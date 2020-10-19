using System;
using System.Diagnostics;
using Unity.Container;
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


        private object? ResolveUnregisteredGeneric(ref Contract contract, ref Contract generic, ResolverOverride[] overrides)
        {
            var request = new RequestInfo(overrides);
            var context = new PipelineContext(this, ref contract, ref request);

            context.Target = ResolveUnregisteredGeneric(ref contract, ref generic, ref context);

            if (context.IsFaulted) throw new ResolutionFailedException(contract.Type, contract.Name, "");

            return context.Target;
        }

        private object? ResolveUnregisteredGeneric(ref Contract contract, ref Contract generic, ref PipelineContext context)
        {
            ResolveDelegate<PipelineContext>? pipeline;

            if (null == (pipeline = _policies.Get<ResolveDelegate<PipelineContext>>(contract.Type)))
            {
                PipelineFactory<Type>? factory;

                if (null == (factory = _policies.Get<PipelineFactory<Type>>(generic.Type)))
                    return ResolveUnregistered(ref contract, ref context);

                var type = contract.Type;
                pipeline = factory(ref type);
            }

            return pipeline(ref context);
        }
    }
}
