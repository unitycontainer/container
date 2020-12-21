using System;
using System.Reflection;

namespace Unity.Extension
{
    /// <summary>
    /// The delegate to define getters for supported members
    /// </summary>
    /// <remarks>
    /// This delegate is used to define methods to retrieve supported members implemented
    /// by the type
    /// </remarks>
    /// <typeparam name="TMemberInfo"><see cref="ConstructorInfo"/>, 
    /// <see cref="FieldInfo"/>, <see cref="PropertyInfo"/>, or <see cref="MethodInfo"/></typeparam>
    /// <param name="type"><see cref="Type"/> implementing the members</param>
    /// <returns>Array of supported members</returns>
    public delegate TMemberInfo[] SupportedMembers<TMemberInfo>(Type type);
}
