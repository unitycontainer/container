using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer
    {
        public static ServiceProducer DefaultProducerFactory(ref ResolutionContext context)
        {
            return context.Container._policies.TypePipeline;

            //throw new NotImplementedException();

            //return context.Container._policies.ResolveContract;
        }

    }
}
