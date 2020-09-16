using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity.Container
{
    public delegate ServiceProducer ProducerFactory(ref ResolutionContext context);


    public interface IProducerFactory
    {
        ServiceProducer Create(ref ResolutionContext context);
    }
}

