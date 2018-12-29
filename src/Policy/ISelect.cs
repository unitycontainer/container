using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Storage;

namespace Unity.Policy
{
    public interface ISelect<TMemberInfo> where TMemberInfo : MemberInfo
    {
        IEnumerable<object> Select(Type type, IPolicySet registration);
    }
}
