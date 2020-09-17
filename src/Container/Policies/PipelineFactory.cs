using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity.Container
{
    public delegate Pipeline PipelineFactory(ref ResolutionContext context);
}

