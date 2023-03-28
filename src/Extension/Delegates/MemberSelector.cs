using Unity.Builder;
using System.Reflection;

namespace Unity.Extension
{
    /// <summary>
    /// The selector method to choose from the list declared members.
    /// This method is used during injection process when algorithm picks member to inject.
    /// </summary>
    /// <typeparam name="TContext">Builder context type</typeparam>
    /// <typeparam name="TMemberInfo"><see cref="ConstructorInfo"/>, <see cref="FieldInfo"/>, 
    /// <see cref="PropertyInfo"/>, or <see cref="MethodInfo"/></typeparam>
    /// <param name="context">Builder context</param>
    /// <param name="members">List of members to choose from</param>
    /// <returns>Selected members</returns>
    public delegate IEnumerable<TMemberInfo>? MemberSelector<TContext, TMemberInfo>(ref TContext context, TMemberInfo[] members)
        where TContext    : IBuilderContext
        where TMemberInfo : class;
}

