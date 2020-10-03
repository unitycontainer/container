using System.Reflection;

namespace Unity.Container
{
    public partial struct InvocationInfo<TInfo>
    {
        public TInfo Info;
        public DependencyInfo<ParameterInfo>[]? Parameters;
    }
}
