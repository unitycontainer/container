using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public partial struct InvokeInfo
        {
            public TMemberInfo Info;
            public InjectionInfo<ParameterInfo>[]? Parameters;

            public InvokeInfo(TMemberInfo info, InjectionInfo<ParameterInfo>[]? parameters = null)
            {
                Info = info;
                Parameters = parameters;
            }
        }
    }
}

