using System;

namespace Unity
{
    /// <summary>
    /// Base class for attributes that can be placed on parameters
    /// or properties to specify how to resolve the value for
    /// that parameter or property.
    /// </summary>
    public abstract class DependencyResolutionAttribute : Attribute
    {
        protected DependencyResolutionAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name specified in the constructor.
        /// </summary>
        public string Name { get; }
    }
}
