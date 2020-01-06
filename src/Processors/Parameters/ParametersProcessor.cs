using System;
using System.Reflection;
using Unity.Builder;
using Unity.Resolution;

namespace Unity.Processors
{
    public abstract partial class ParametersProcessor<TMemberInfo> : MemberProcessor<TMemberInfo, object[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Fields
        
        protected const string InvalidArgument = "Invalid Argument";

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<BuilderContext>? PreProcessResolver(ParameterInfo info, DependencyResolutionAttribute attribute, object? data) 
            => data switch
        {
            IResolve policy                                   => policy.Resolve,
            IResolverFactory<ParameterInfo> fieldFactory      => fieldFactory.GetResolver<BuilderContext>(info),
            IResolverFactory<Type> typeFactory                => typeFactory.GetResolver<BuilderContext>(info.ParameterType),
            Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<BuilderContext>(type),
            _                                                 => null
        };

        #endregion
    }
}
