using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        public partial struct InvokeInfo
        {
            public TMemberInfo Info;
            public ReflectionInfo<ParameterInfo>[]? Parameters;

            public InvokeInfo(TMemberInfo info, ReflectionInfo<ParameterInfo>[]? parameters = null)
            {
                Info = info;
                Parameters = parameters;
            }
        }

    }

    public static class ParameterProcessorExtensions
    {
        public static ParameterProcessor<TMemberInfo>.InvokeInfo AsInvokeInfo<TMemberInfo>(this TMemberInfo info)
            where TMemberInfo : MethodBase
        {
            var parameters = info.GetParameters();
            var arguments = new ReflectionInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = parameters[i].AsInjectionInfo();

            return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }

        public static ParameterProcessor<TMemberInfo>.InvokeInfo AsInvokeInfo<TMemberInfo>(this TMemberInfo info, object?[]? data)
            where TMemberInfo : MethodBase
        {
            var parameters = info.GetParameters();
            var arguments = new ReflectionInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = parameters[i].AsInjectionInfo(data![i]);

            return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }
    }
}

