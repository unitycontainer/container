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

        /// <summary>
        /// Global singleton containing empty parameter array
        /// </summary>
        protected static object?[] EmptyParametersArray = new object?[0];

        /// <summary>
        /// Delegate holding parameter dependency analizer
        /// </summary>
        protected DependencyAnalyzer<ParameterInfo> GetParameterInfo { get; private set; }


        #endregion


        #region Constructors

        public ParameterProcessor(Defaults defaults)
            : base(defaults)
        {
            GetParameterInfo = defaults
                .GetOrAdd<DependencyAnalyzer<ParameterInfo>>(OnGetParameterInfo, (object handler) => GetParameterInfo = (DependencyAnalyzer<ParameterInfo>)handler);
        }

        #endregion


        #region Dependency Management

        public virtual DependencyInfo OnGetParameterInfo(ParameterInfo memberInfo, object? data)
        {
            return default;
        }
        
        #endregion


        #region Pre Processor

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
