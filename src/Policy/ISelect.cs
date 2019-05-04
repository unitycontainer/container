using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Policy
{
    // TODO: Replace with delegate
    public interface ISelect<TMemberInfo> where TMemberInfo : MemberInfo
    {
        IEnumerable<object> Select(Type type, IPolicySet registration);
    }
}
