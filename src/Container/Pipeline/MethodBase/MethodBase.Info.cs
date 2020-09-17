using System;
using System.Reflection;

namespace Unity.Container
{
    public abstract partial class MethodBaseProcessor<TMemberInfo> : MemberInfoProcessor<TMemberInfo, object?[]>
                                                 where TMemberInfo : MethodBase
    {
        #region Parameter Dependency Info

        protected virtual DependencyInfo OnGetParameterDependencyInfo(ParameterInfo info)
        {
            var attribute = info.GetCustomAttribute(typeof(DependencyResolutionAttribute)) as DependencyResolutionAttribute
                                                                                           ?? DependencyAttribute.Instance;
            // TODO: Default values???

            return new DependencyInfo(attribute.Name, attribute);
        }

        #endregion


        #region OnChange handlers

        private void OnGetParameterDependencyInfoChanged(object policy) => GetParameterDependencyInfo = (Func<ParameterInfo, DependencyInfo>)policy;
        
        #endregion
    }
}
