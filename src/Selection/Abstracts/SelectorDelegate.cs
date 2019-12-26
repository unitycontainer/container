using System.Reflection;
using Unity.Injection;

namespace Unity.Selection
{
    public delegate TReturn SelectorDelegate<in TMember, in TParam, out TReturn>(MemberInfo[] members, InjectionMember[]? injectors)
        where TMember : MemberInfo;
}
