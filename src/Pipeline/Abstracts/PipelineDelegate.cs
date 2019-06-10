using System.Threading.Tasks;
using Unity;

namespace Unity
{
    public delegate ValueTask<object?> PipelineDelegate(ref PipelineContext context);
}
