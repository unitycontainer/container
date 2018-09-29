using System;
using Unity.Policy;

namespace Unity.Build
{
    public interface IBuildContext// : IPolicyList
    {
        /// <summary>Reference to container.</summary>
        /// <remarks>Reference to the container used to execute this build. </remarks>
        /// <returns> Interface for the hosting container</returns>
        IUnityContainer Container { get; }

        /// <summary>
        /// <see cref="Type"/>  to build.
        /// </summary>
        /// <remarks>This is the type that is being created. If, for example,
        /// parameter is being resolved this member will refer Method/Constructor
        /// info of the parameter</remarks>
        Type Type { get; }

        /// <summary>
        /// Name of the registration.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The current object being built up or resolved.
        /// </summary>
        /// <value>
        /// The current object being manipulated by the build operation. May
        /// be null if the object hasn't been created yet.</value>
        object Existing { get; set; }

        ///// <summary>
        ///// Resolve type/object/dependency using current context
        ///// </summary>
        ///// <param name="type">Type of requested object</param>
        ///// <param name="name">Name of registration</param>
        ///// <returns></returns>
        //object Resolve(Type type, string name);

        //object Resolve(PropertyInfo property, string name, object value = null);

        //object Resolve(ParameterInfo parameter, string name, object value = null);
    }
}
