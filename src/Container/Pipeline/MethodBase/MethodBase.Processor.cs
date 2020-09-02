using System;
using System.Reflection;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public abstract partial class MethodBaseProcessor<TMemberInfo> : MemberInfoProcessor<TMemberInfo, object?[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Fields

        protected static object?[] EmptyParametersArray = new object?[0];

        #endregion


        #region Constructors

        public MethodBaseProcessor(Defaults defaults)
            :base(defaults)
        {
            GetParameterDependencyInfo = (Func<ParameterInfo, DependencyInfo>?)defaults.Get(typeof(TMemberInfo), typeof(Func<ParameterInfo, DependencyInfo>))!;
            if (null == GetParameterDependencyInfo)
            {
                GetParameterDependencyInfo = OnGetParameterDependencyInfo;
                defaults.Set(typeof(TMemberInfo), typeof(Func<ParameterInfo, DependencyInfo>), GetParameterDependencyInfo, OnGetParameterDependencyInfoChanged);
            }
        }

        #endregion


        #region Public API

        protected Func<ParameterInfo, DependencyInfo> GetParameterDependencyInfo { get; private set; }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<ResolutionContext>? PreProcessResolver(ParameterInfo info, DependencyResolutionAttribute attribute, object? data)
                    => data switch
                    {
                        IResolve policy                                   => policy.Resolve,
                        IResolverFactory<ParameterInfo> fieldFactory      => fieldFactory.GetResolver<ResolutionContext>(info),
                        IResolverFactory<Type> typeFactory                => typeFactory.GetResolver<ResolutionContext>(info.ParameterType),
                        Type type when typeof(Type) != info.ParameterType => attribute.GetResolver<ResolutionContext>(type),
                        _ => null
                    };

        protected virtual object? ProcessResolver(ref ResolutionContext context, ref DependencyInfo info, object? data)
        {
            var parameter = (ParameterInfo)context.Data!;

            return data switch
            {
                IResolve policy                                        => policy.Resolve(ref context),
                IResolverFactory<ParameterInfo> fieldFactory           => fieldFactory.GetResolver<ResolutionContext>(parameter)(ref context),
                IResolverFactory<Type> typeFactory                     => typeFactory.GetResolver<ResolutionContext>(parameter.ParameterType)(ref context),
                
                Type type when typeof(Type) != parameter.ParameterType && 
                info.Data is DependencyResolutionAttribute attribute   => attribute.GetResolver<ResolutionContext>(type)(ref context),
                
                _ => RegistrationManager.NoValue
            };
        }

        #endregion

    }
}
