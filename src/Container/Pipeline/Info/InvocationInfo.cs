using System.Reflection;

namespace Unity.Container
{
    public partial struct InvocationInfo<TInfo>
    {
        public TInfo Info;
        public DependencyInfo<ParameterInfo>[]? Parameters;

        public InvocationInfo(TInfo info, DependencyInfo<ParameterInfo>[]? parameters = null)
        {
            Info = info;
            Parameters = parameters;
        }
    }
}
