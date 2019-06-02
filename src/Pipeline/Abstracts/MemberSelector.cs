using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Injection;

namespace Unity
{
    public delegate IEnumerable<object> MemberSelector<TMember>(Type type, InjectionMember[]? members)
        where TMember : MemberInfo;
}
