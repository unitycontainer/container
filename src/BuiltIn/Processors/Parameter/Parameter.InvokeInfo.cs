using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo>
    {
        [CLSCompliant(false)]
        protected static ParameterProcessor<TMemberInfo>.InvokeInfo ToInvokeInfo(TMemberInfo info)
        {
            var parameters = info.GetParameters();
            var arguments = new ReflectionInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = ToInjectionInfo(parameters[i]);

            return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }

        [CLSCompliant(false)]
        protected static ParameterProcessor<TMemberInfo>.InvokeInfo ToInvokeInfo(TMemberInfo info, object?[]? data)
        {
            var parameters = info.GetParameters();
            var arguments = new ReflectionInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                arguments[i] = ToInjectionInfoFromData(parameters[i], data![i]);

            return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }

        #region InvokeInfo

        protected struct InvokeInfo
        {
            public TMemberInfo Info;
            public ReflectionInfo<ParameterInfo>[]? Parameters;

            public InvokeInfo(TMemberInfo info, ReflectionInfo<ParameterInfo>[]? parameters = null)
            {
                Info = info;
                Parameters = parameters;
            }
        }

        #endregion
    }
}

