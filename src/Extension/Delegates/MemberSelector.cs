using System.Reflection;
using Unity.Injection;

namespace Unity.Extension
{
    /// <summary>
    /// The selector that depends on the current container's configuration
    /// </summary>
    /// <typeparam name="TMemberInfo"><see cref="Type"/> of the member</typeparam>
    /// <typeparam name="TData"><see cref="Type"/> of the data</typeparam>
    /// <param name="member">Instance of the container</param>
    /// <param name="members">Value[s] to select from</param>
    /// <returns>Selected value</returns>
    public delegate int MemberSelector<TMemberInfo, TData>(InjectionMember<TMemberInfo, TData> member, TMemberInfo[] members, ref Span<int> indexes)
        where TMemberInfo : MemberInfo
        where TData : class;
}

