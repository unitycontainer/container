using System.Reflection;

namespace Unity.Container
{
    public readonly partial struct InvokeInfo<TInfo>
    {
        public readonly TInfo Info;
        public readonly InjectionInfo<ParameterInfo>[]? Parameters;

        public InvokeInfo(TInfo info, InjectionInfo<ParameterInfo>[]? parameters)
        {
            Info = info;
            Parameters = parameters;
        }

        public InvokeInfo(TInfo info)
        {
            Info = info;
            Parameters = null;
        }
    }
}
