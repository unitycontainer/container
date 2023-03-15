using System;


namespace Unity
{
    /// <summary>
    /// Base class for attributes that can be placed on parameters
    /// or properties to specify how to resolve the value for
    /// that parameter or property.
    /// </summary>
    public abstract class DependencyResolutionAttribute : ImportAttribute
    {
        #region Constructors


        protected DependencyResolutionAttribute(Type type, string name)
            : base(name, type)
        {
        }

        protected DependencyResolutionAttribute(Type type)
            : base(type)
        {
        }

        protected DependencyResolutionAttribute(string name)
            :base(name)
        {
        }

        protected DependencyResolutionAttribute()
            : base()
        {
        }

        #endregion


        #region Public Members

        /// <summary>
        /// The name specified in the constructor.
        /// </summary>
        public string? Name => ContractName;

        #endregion
    }
}
