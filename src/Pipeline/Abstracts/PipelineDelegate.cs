using System.Threading.Tasks;
using Unity.Builder;

namespace Unity
{
    public delegate ValueTask<object?> PipelineDelegate(ref BuilderContext context);
}
