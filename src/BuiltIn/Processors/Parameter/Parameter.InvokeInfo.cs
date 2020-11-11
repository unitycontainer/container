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
            throw new NotImplementedException();
            //var parameters = info.GetParameters();
            //var arguments = new ReflectionInfo<ParameterInfo>[parameters.Length];

            ////for (var i = 0; i < parameters.Length; i++)
            ////    arguments[i] = ToInjectionInfo(parameters[i]);

            //return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }

        [CLSCompliant(false)]
        protected static ParameterProcessor<TMemberInfo>.InvokeInfo ToInvokeInfo(TMemberInfo info, object?[]? data)
        {
            throw new NotImplementedException();
            //var parameters = info.GetParameters();
            //var arguments = new ReflectionInfo<ParameterInfo>[parameters.Length];

            ////for (var i = 0; i < parameters.Length; i++)
            ////    arguments[i] = ToInjectionInfoFromData(parameters[i], data![i]);

            //return new ParameterProcessor<TMemberInfo>.InvokeInfo(info, arguments);
        }

        #region InvokeInfo

        protected struct InvokeInfo
        {
            public TMemberInfo Info;

        }

        #endregion
    }
}

