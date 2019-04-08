using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity.Abstracts
{
    public delegate Task<object> PipelineDelegate(IUnityContainer container, params ResolverOverride[] overrides);
}
