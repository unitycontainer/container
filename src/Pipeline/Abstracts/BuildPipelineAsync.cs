using System.Threading.Tasks;

namespace Unity
{
    public delegate ValueTask<object?> BuildPipelineAsync(ref PipelineContext context);
}
