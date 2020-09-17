using System.Threading.Tasks;
using Unity.Resolution;

namespace Unity.Container
{
    public delegate object? Pipeline(ref ResolutionContext context);
}

