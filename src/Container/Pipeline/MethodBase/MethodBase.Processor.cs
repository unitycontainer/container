using System;
using System.Reflection;
using Unity.Container;

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


        #endregion


        #region OnChange handlers

        
        #endregion
    }
}
