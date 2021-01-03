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
    /// <example>
    /// For example this is how default constructors selector is registered:
    /// <code>
    /// defaults.Set<SupportedMembers<ConstructorInfo>>((type) => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance));
    /// </code>
    /// </example>
    /// <returns>Array of supported members</returns>
    public delegate TMemberInfo[] MembersDelegate<TMemberInfo>(Type type);
}
