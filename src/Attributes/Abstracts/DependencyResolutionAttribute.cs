using System;
using Unity.Injection;

namespace Unity
{
    /// <summary>
    /// Base class for attributes that can be placed on parameters
    /// or properties to specify how to resolve the value for
    /// that parameter or property.
    /// </summary>
    public abstract class DependencyResolutionAttribute : Attribute, IMatch<Type>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">Name of the registration to resolve</param>
        protected DependencyResolutionAttribute(string? name)
        {
            Name = name;
        }

        /// <summary>
        /// The name specified in the constructor.
        /// </summary>
        public string? Name { get; }


        /// <summary>
        /// This method matches type to dependency and checks if it can be resolved
        /// </summary>
        /// <param name="type">Type to resolve</param>
        /// <returns>Returns true in assumption that any time can be resolved</returns>
        public virtual bool Match(Type type) => true;
    }
}
