using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public abstract partial class ParameterProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Fields

        protected static object?[] EmptyParametersArray = new object?[0];

        #endregion


        #region Constructors

        public ParameterProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<PipelineContext>? PreProcessResolver(ParameterInfo info, DependencyResolutionAttribute attribute, object? data)
            => data switch
            {
                IResolve policy => policy.Resolve,
                IResolverFactory<ParameterInfo> fieldFactory => fieldFactory.GetResolver<PipelineContext>(info),
                IResolverFactory<Type> typeFactory => typeFactory.GetResolver<PipelineContext>(info.ParameterType),
                Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<PipelineContext>(type),
                _ => null
            };

        #endregion

    }
}
