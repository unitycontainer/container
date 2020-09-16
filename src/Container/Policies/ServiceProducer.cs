using System;
using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity.Container
{ 
    public delegate ValueTask<object?> ServiceProducer(ref ResolutionContext context);


    public interface IServiceProducer
    {
        ValueTask<object?> ProduceService(ref ResolutionContext context);
    }
}

