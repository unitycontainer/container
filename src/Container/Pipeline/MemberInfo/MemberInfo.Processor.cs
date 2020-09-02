using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Pipeline
{
    public abstract partial class MemberInfoProcessor<TMemberInfo, TData> : PipelineProcessor
                                                        where TMemberInfo : MemberInfo
                                                        where TData       : class
    {
        #region Constructors

        public MemberInfoProcessor(Defaults defaults) 
        {
            // Add BindingFlags to default policies and subscribe to notifications
            var flags = defaults.Get(typeof(TMemberInfo), typeof(BindingFlags));
            if (null == flags)
            {
                BindingFlags = BindingFlags.Public | BindingFlags.Instance;
                defaults.Set(typeof(TMemberInfo), typeof(BindingFlags), BindingFlags, OnBindingFlagsChanged);
            }
            else
            {
                BindingFlags = (BindingFlags)flags;
            }

            GetDependencyInfo = (Func<TMemberInfo, DependencyInfo>?)defaults.Get(typeof(TMemberInfo), typeof(Func<TMemberInfo, DependencyInfo>))!;
            if (null == GetDependencyInfo)
            {
                GetDependencyInfo = OnGetDependencyInfo;
                defaults.Set(typeof(TMemberInfo), typeof(Func<TMemberInfo, DependencyInfo>), GetDependencyInfo, OnGetDependencyInfoChanged);
            }
        }

        #endregion


        #region Implementation

        private void OnBindingFlagsChanged(object policy) => BindingFlags = (BindingFlags)policy;

        private void OnGetDependencyInfoChanged(object policy) => GetDependencyInfo = (Func<TMemberInfo, DependencyInfo>)policy;

        #endregion
    }
}
