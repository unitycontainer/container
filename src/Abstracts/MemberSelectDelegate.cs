using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Policy;

namespace Unity
{
    public delegate IEnumerable<object> MemberSelectDelegate<TMember>(Type type, IPolicySet set)
        where TMember : MemberInfo;
}
