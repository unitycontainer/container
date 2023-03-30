using System.Reflection;
using Unity.Builder;

namespace Unity.Extension
{
    /// <summary>
    /// The selector method to choose from the list declared members.
    /// This method is used during injection process when algorithm picks member to inject.
    /// </summary>
    /// <typeparam name="TContext">Builder context type</typeparam>
    /// <typeparam name="TMemberInfo"><see cref="ConstructorInfo"/>, <see cref="FieldInfo"/>, 
    /// <see cref="PropertyInfo"/>, or <see cref="MethodInfo"/></typeparam>
    /// <typeparam name="TSelect"><see cref="Type"/> of selected value</typeparam>
    /// <param name="context">Builder context</param>
    /// <param name="members">List of members to choose from</param>
    /// <returns>Selected members</returns>
    public delegate TSelect MemberSelector<TContext, in TMemberInfo, out TSelect>(ref TContext context, TMemberInfo[] members)
        where TContext    : IBuilderContext
        where TMemberInfo : class;
}

