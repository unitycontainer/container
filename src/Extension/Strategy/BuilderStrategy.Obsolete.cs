using System;
using System.ComponentModel;

namespace Unity.Extension
{
    public abstract partial class BuilderStrategy
    {
        #region v4

        /// <summary>
        /// Obsolete PreTearDown. No longer supported
        /// </summary>
        [Obsolete("PreTearDown method is deprecated. Tear down no longer supported", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PreTearDown(IBuilderContext context)
        {
        }

        /// <summary>
        /// Obsolete PostTearDown. No longer supported
        /// </summary>
        [Obsolete("PostTearDown method is deprecated. Tear down no longer supported", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void PostTearDown(IBuilderContext context)
        {
        }

        #endregion


        #region v5

        /// <summary>
        /// Obsolete RequiredToBuildType. No longer supported
        /// </summary>
        [Obsolete("RequiredToBuildType method is deprecated", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool RequiredToBuildType(IUnityContainer container, Type type)
        {
            return true;
        }


        /// <summary>
        /// Obsolete RequiredToResolveInstance. No longer supported
        /// </summary>
        [Obsolete("RequiredToResolveInstance method is deprecated", true)]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool RequiredToResolveInstance(IUnityContainer container)
        {
            return false;
        }

        #endregion
    }
}
