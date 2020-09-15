using System;
using System.Threading.Tasks;

namespace Unity
{ 
    public delegate ValueTask<object?> ServiceProducer<TContext>(ref TContext context);


    public interface IProduceService
    {
        ValueTask<object?> ProduceService<TContext>(ref TContext context)
            where TContext : struct;
    }
}

