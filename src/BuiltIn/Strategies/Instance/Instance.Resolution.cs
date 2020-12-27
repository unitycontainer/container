using System;
using Unity.Extension;

namespace Unity.Container
{
    public partial class InstanceProcessor
    {
        public override ResolveDelegate<PipelineContext>? Build(ref Pipeline_Builder<ResolveDelegate<PipelineContext>?> builder)
        {
            throw new NotImplementedException();
            //

            //// Skip if already have a resolver
            //if (null != builder.SeedMethod) return builder.Pipeline();

            //var registration = builder.Registration as FactoryRegistration ??
            //                   builder.Factory      as FactoryRegistration;

            //Debug.Assert(null != registration);
            //var factory = registration!.Factory;

            //return builder.Pipeline((ref PipelineContext context) => factory(context));
        }
    }
}
