using System;

namespace Unity.Extension
{
    public abstract partial class BuilderStrategy
    {
        #region v4

        /// <summary>
        /// Tear-down is no longer supported by the container. Use <see cref="IDisposable"/>
        /// for proper deactivation of components
        /// </summary>
        [Obsolete("PreTearDown method is deprecated. Tear down no longer supported", true)]
        public virtual void PreTearDown(IBuilderContext context)
        {
        }

        /// <summary>
        /// Tear-down is no longer supported by the container. Use <see cref="IDisposable"/>
        /// for proper deactivation of components
        /// </summary>
        [Obsolete("PostTearDown method is deprecated. Tear down no longer supported", true)]
        public virtual void PostTearDown(IBuilderContext context)
        {
        }

        #endregion

        #region v5

        /// <summary>
        /// Analysis of types is no longer performed during registration
        /// </summary>
        [Obsolete("RequiredToBuildType method is deprecated", true)]
        public virtual bool RequiredToBuildType(IUnityContainer container, Type type)
        {
            return true;
        }

        /// <summary>
        /// Analysis of types is no longer performed during registration
        /// </summary>
        [Obsolete("RequiredToResolveInstance method is deprecated", true)]
        public virtual bool RequiredToResolveInstance(IUnityContainer container)
        {
            return false;
        }

        #endregion
    }
}
